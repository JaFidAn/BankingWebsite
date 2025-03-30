using Application.Core;
using Application.DTOs.Transactions;

namespace Application.Services;

public interface ITransactionService
{
    Task<Result<string>> CreateAsync(string userId, CreateTransactionDto dto);
    Task<Result<TransactionDto>> GetByIdAsync(string id);
    Task<Result<PagedResult<TransactionDto>>> GetAllByAccountIdAsync(string accountId, PaginationParams paginationParams, CancellationToken cancellationToken);
    Task<Result<PagedResult<TransactionDto>>> GetAllByUserIdAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken);
}
