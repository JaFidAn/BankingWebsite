using Application.Core;
using Application.DTOs.Transactions;
using Application.Repositories.AccountRepositories;
using Application.Repositories.TransactionRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
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
    private readonly IEncryptionService _encryptionService;

    public TransactionService(
        ITransactionWriteRepository writeRepository,
        ITransactionReadRepository readRepository,
        IAccountReadRepository accountReadRepository,
        IAccountWriteRepository accountWriteRepository,
        IMapper mapper,
        IEncryptionService encryptionService)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _accountReadRepository = accountReadRepository;
        _accountWriteRepository = accountWriteRepository;
        _mapper = mapper;
        _encryptionService = encryptionService;
    }

    public async Task<Result<string>> CreateAsync(string userId, CreateTransactionDto dto)
    {
        if (!Enum.TryParse(dto.Type, out TransactionType type))
            return Result<string>.Failure("Invalid transaction type", 400);

        Account? fromAccount = null;
        if (!string.IsNullOrWhiteSpace(dto.FromAccountId))
        {
            fromAccount = await _accountReadRepository.GetByIdAsync(dto.FromAccountId);
            if (fromAccount == null || fromAccount.UserId != userId || fromAccount.IsDeleted)
                return Result<string>.Failure("FromAccount not found or not owned by user", 404);
        }

        Account? toAccount = null;
        if (!string.IsNullOrWhiteSpace(dto.ToAccountId))
        {
            toAccount = await _accountReadRepository.GetByIdAsync(dto.ToAccountId);
            if (toAccount == null || toAccount.IsDeleted)
                return Result<string>.Failure("ToAccount not found", 404);
        }

        if ((type == TransactionType.Transfer || type == TransactionType.Withdraw) && (fromAccount == null || fromAccount.Balance < dto.Amount))
            return Result<string>.Failure("Insufficient funds", 400);

        var isSuspicious = await CheckForSuspicionAsync(userId, dto, type);

        var transaction = new Transaction
        {
            FromAccountId = fromAccount?.Id,
            ToAccountId = toAccount?.Id,
            Amount = dto.Amount,
            Type = type,
            Status = TransactionStatus.Success,
            Description = dto.Description,
            Note = string.IsNullOrWhiteSpace(dto.Note) ? null : _encryptionService.Encrypt(dto.Note),
            IsSuspicious = isSuspicious
        };

        if (type == TransactionType.Withdraw || type == TransactionType.Transfer)
        {
            fromAccount!.Balance -= dto.Amount;
            _accountWriteRepository.Update(fromAccount);
        }

        if (type == TransactionType.Deposit && toAccount != null)
        {
            toAccount.Balance += dto.Amount;
            _accountWriteRepository.Update(toAccount);
        }

        if (type == TransactionType.Transfer && toAccount != null)
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
        if (!string.IsNullOrWhiteSpace(transaction.Note))
            dto.Note = _encryptionService.Decrypt(transaction.Note);

        return Result<TransactionDto>.Success(dto);
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetAllByAccountIdAsync(string accountId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var query = _readRepository
            .GetAll()
            .Where(x => x.FromAccountId == accountId || x.ToAccountId == accountId)
            .OrderByDescending(x => x.Timestamp)
            .Select(x => new TransactionDto
            {
                Id = x.Id,
                FromAccountId = x.FromAccountId,
                ToAccountId = x.ToAccountId,
                Amount = x.Amount,
                Type = x.Type.ToString(),
                Status = x.Status.ToString(),
                Description = x.Description,
                Timestamp = x.Timestamp,
                Note = string.IsNullOrWhiteSpace(x.Note) ? null : _encryptionService.Decrypt(x.Note)
            });

        var paged = await PagedResult<TransactionDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<TransactionDto>>.Success(paged);
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetAllByUserIdAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var accountIds = _accountReadRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .Select(x => x.Id);

        var query = _readRepository
            .GetAll()
            .Where(x => accountIds.Contains(x.FromAccountId!) || accountIds.Contains(x.ToAccountId!))
            .OrderByDescending(x => x.Timestamp)
            .Select(x => new TransactionDto
            {
                Id = x.Id,
                FromAccountId = x.FromAccountId,
                ToAccountId = x.ToAccountId,
                Amount = x.Amount,
                Type = x.Type.ToString(),
                Status = x.Status.ToString(),
                Description = x.Description,
                Timestamp = x.Timestamp,
                Note = string.IsNullOrWhiteSpace(x.Note) ? null : _encryptionService.Decrypt(x.Note)
            });

        var paged = await PagedResult<TransactionDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<TransactionDto>>.Success(paged);
    }

    public async Task<Result<List<TransactionDto>>> GetSuspiciousAsync()
    {
        var transactions = await _readRepository
            .GetAll()
            .Where(t => t.IsSuspicious)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();

        var dtos = _mapper.Map<List<TransactionDto>>(transactions);

        foreach (var (dto, entity) in dtos.Zip(transactions))
        {
            if (!string.IsNullOrWhiteSpace(entity.Note))
            {
                dto.Note = _encryptionService.Decrypt(entity.Note);
            }
        }

        return Result<List<TransactionDto>>.Success(dtos);
    }

    private async Task<bool> CheckForSuspicionAsync(string userId, CreateTransactionDto dto, TransactionType type)
    {
        if (dto.Amount > 10000)
            return true;

        if (string.IsNullOrWhiteSpace(dto.FromAccountId))
            return false;

        var fromAccountId = dto.FromAccountId!;
        var now = DateTime.UtcNow;

        var today = now.Date;
        var countToday = await _readRepository
            .GetAll()
            .CountAsync(x => x.FromAccountId == fromAccountId &&
                             x.Timestamp >= today && x.Timestamp < today.AddDays(1));

        if (countToday > 5)
            return true;

        if (!string.IsNullOrWhiteSpace(dto.ToAccountId))
        {
            var thirtyMinutesAgo = now.AddMinutes(-30);

            var recentTransfers = await _readRepository
                .GetAll()
                .Where(x => x.FromAccountId == fromAccountId &&
                            x.ToAccountId == dto.ToAccountId &&
                            x.Timestamp >= thirtyMinutesAgo)
                .CountAsync();

            if (recentTransfers >= 3)
                return true;
        }

        return false;
    }
}
