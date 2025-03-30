using Application.Core;
using Application.DTOs.Payments;

namespace Application.Services;

public interface IStripePaymentService
{
    Task<Result<PaymentResponseDto>> ProcessPaymentAsync(PaymentRequestDto dto, string userId);
}
