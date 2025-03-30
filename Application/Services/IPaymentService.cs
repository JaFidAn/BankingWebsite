using Application.Core;
using Application.DTOs.Payments;

namespace Application.Services;

public interface IPaymentService
{
    Task<Result<string>> CreateAsync(CreatePaymentDto dto, string userId);
    Task<Result<PaymentDto>> GetByIdAsync(string id);
    Task<Result<PagedResult<PaymentDto>>> GetAllByUserAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken);
}
