using Application.Core;
using Application.DTOs.Transactions;
using Application.Repositories.AccountRepositories;
using Application.Repositories.TransactionRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionWriteRepository _writeRepository;
    private readonly ITransactionReadRepository _readRepository;
    private readonly IAccountReadRepository _accountReadRepository;
    private readonly IAccountWriteRepository _accountWriteRepository;
    private readonly IMapper _mapper;

    public TransactionService(
        ITransactionWriteRepository writeRepository,
        ITransactionReadRepository readRepository,
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

    public async Task<Result<string>> CreateAsync(string userId, CreateTransactionDto dto)
    {
        if (!Enum.TryParse(dto.Type, out TransactionType type))
            return Result<string>.Failure("Invalid transaction type", 400);

        var fromAccount = await _accountReadRepository.GetByIdAsync(dto.FromAccountId);
        if (fromAccount == null || fromAccount.UserId != userId || fromAccount.IsDeleted)
            return Result<string>.Failure("FromAccount not found or not owned by user", 404);

        Account? toAccount = null;

        if (type == TransactionType.Transfer)
        {
            if (string.IsNullOrWhiteSpace(dto.ToAccountId))
                return Result<string>.Failure("ToAccountId is required", 400);

            toAccount = await _accountReadRepository.GetByIdAsync(dto.ToAccountId!);
            if (toAccount == null || toAccount.IsDeleted)
                return Result<string>.Failure("ToAccount not found", 404);
        }

        if ((type == TransactionType.Transfer || type == TransactionType.Withdraw) && fromAccount.Balance < dto.Amount)
            return Result<string>.Failure("Insufficient funds", 400);

        var transaction = new Transaction
        {
            FromAccountId = dto.FromAccountId,
            ToAccountId = dto.ToAccountId,
            Amount = dto.Amount,
            Type = type,
            Status = TransactionStatus.Success,
            Description = dto.Description
        };

        if (type == TransactionType.Withdraw || type == TransactionType.Transfer)
        {
            fromAccount.Balance -= dto.Amount;
            _accountWriteRepository.Update(fromAccount);
        }

        if (type == TransactionType.Deposit)
        {
            fromAccount.Balance += dto.Amount;
            _accountWriteRepository.Update(fromAccount);
        }

        if (type == TransactionType.Transfer && toAccount is not null)
        {
            toAccount.Balance += dto.Amount;
            _accountWriteRepository.Update(toAccount);
        }

        await _writeRepository.AddAsync(transaction);
        await _writeRepository.SaveAsync();

        return Result<string>.Success(transaction.Id, MessageGenerator.CreationSuccess("Transaction"));
    }

    public async Task<Result<TransactionDto>> GetByIdAsync(string id)
    {
        var transaction = await _readRepository.GetByIdAsync(id);
        if (transaction == null)
            return Result<TransactionDto>.Failure(MessageGenerator.NotFound("Transaction"), 404);

        var dto = _mapper.Map<TransactionDto>(transaction);
        return Result<TransactionDto>.Success(dto);
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetAllByAccountIdAsync(string accountId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var query = _readRepository
            .GetAll()
            .Where(x => x.FromAccountId == accountId || x.ToAccountId == accountId)
            .OrderByDescending(x => x.Timestamp)
            .ProjectTo<TransactionDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<TransactionDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<TransactionDto>>.Success(pagedResult);
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetAllByUserIdAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var accountIds = _accountReadRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .Select(x => x.Id);

        var query = _readRepository
            .GetAll()
            .Where(x => accountIds.Contains(x.FromAccountId) || accountIds.Contains(x.ToAccountId!))
            .OrderByDescending(x => x.Timestamp)
            .ProjectTo<TransactionDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<TransactionDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<TransactionDto>>.Success(pagedResult);
    }
}
