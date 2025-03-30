using Application.Core;
using Application.DTOs.Accounts;

namespace Application.Services;

public interface IAccountService
{
    Task<Result<AccountDto>> GetByIdAsync(string id);
    Task<Result<PagedResult<AccountDto>>> GetAllAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken);
    Task<Result<string>> CreateAsync(string userId, CreateAccountDto dto);
    Task<Result<bool>> UpdateAsync(string id, UpdateAccountDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}
