using Application.Core;
using Application.DTOs.Payments;
using Application.Repositories.AccountRepositories;
using Application.Repositories.PaymentRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentWriteRepository _writeRepository;
    private readonly IPaymentReadRepository _readRepository;
    private readonly IAccountReadRepository _accountReadRepository;
    private readonly IAccountWriteRepository _accountWriteRepository;
    private readonly IMapper _mapper;

    public PaymentService(
        IPaymentWriteRepository writeRepository,
        IPaymentReadRepository readRepository,
        IAccountReadRepository accountReadRepository,
        IAccountWriteRepository accountWriteRepository,
        IMapper mapper)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _accountReadRepository = accountReadRepository;
        _accountWriteRepository = accountWriteRepository;
        _mapper = mapper;
    }

    public async Task<Result<string>> CreateAsync(CreatePaymentDto dto, string userId)
    {
        var account = await _accountReadRepository.GetByIdAsync(dto.AccountId);
        if (account == null || account.UserId != userId || account.IsDeleted)
            return Result<string>.Failure("Account not found or not owned by user", 404);

        var payment = _mapper.Map<Payment>(dto);
        payment.Status = PaymentStatus.Success;

        account.Balance += dto.Amount;

        _accountWriteRepository.Update(account);
        await _writeRepository.AddAsync(payment);
        await _writeRepository.SaveAsync();

        return Result<string>.Success(payment.Id, MessageGenerator.CreationSuccess("Payment"));
    }

    public async Task<Result<PaymentDto>> GetByIdAsync(string id)
    {
        var payment = await _readRepository.GetByIdAsync(id);
        if (payment == null)
            return Result<PaymentDto>.Failure(MessageGenerator.NotFound("Payment"), 404);

        var dto = _mapper.Map<PaymentDto>(payment);
        return Result<PaymentDto>.Success(dto);
    }

    public async Task<Result<PagedResult<PaymentDto>>> GetAllByUserAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var accountIds = _accountReadRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .Select(x => x.Id);

        var query = _readRepository
            .GetAll()
            .Where(x => accountIds.Contains(x.AccountId))
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<PaymentDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<PaymentDto>>.Success(pagedResult);
    }
}
