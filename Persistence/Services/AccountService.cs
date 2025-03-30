using Application.Core;
using Application.DTOs.Accounts;
using Application.Repositories.AccountRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;

namespace Persistence.Services;

public class AccountService : IAccountService
{
    private readonly IAccountReadRepository _readRepository;
    private readonly IAccountWriteRepository _writeRepository;
    private readonly IMapper _mapper;

    public AccountService(
        IAccountReadRepository readRepository,
        IAccountWriteRepository writeRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _mapper = mapper;
    }

    public async Task<Result<AccountDto>> GetByIdAsync(string id)
    {
        var account = await _readRepository.GetByIdAsync(id);
        if (account == null || account.IsDeleted)
            return Result<AccountDto>.Failure(MessageGenerator.NotFound("Account"), 404);

        var dto = _mapper.Map<AccountDto>(account);
        return Result<AccountDto>.Success(dto);
    }

    public async Task<Result<PagedResult<AccountDto>>> GetAllAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var query = _readRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<AccountDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<AccountDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<AccountDto>>.Success(pagedResult);
    }

    public async Task<Result<string>> CreateAsync(string userId, CreateAccountDto dto)
    {
        var account = _mapper.Map<Account>(dto);
        account.UserId = userId;

        await _writeRepository.AddAsync(account);
        await _writeRepository.SaveAsync();

        return Result<string>.Success(account.Id, MessageGenerator.CreationSuccess("Account"));
    }

    public async Task<Result<bool>> UpdateAsync(string id, UpdateAccountDto dto)
    {
        var account = await _readRepository.GetByIdAsync(id);
        if (account == null || account.IsDeleted)
            return Result<bool>.Failure(MessageGenerator.NotFound("Account"), 404);

        _mapper.Map(dto, account);
        _writeRepository.Update(account);
        await _writeRepository.SaveAsync();

        return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Account"));
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        var account = await _readRepository.GetByIdAsync(id);
        if (account == null || account.IsDeleted)
            return Result<bool>.Failure(MessageGenerator.NotFound("Account"), 404);

        account.IsDeleted = true;
        _writeRepository.Update(account);
        await _writeRepository.SaveAsync();

        return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Account"));
    }
}
