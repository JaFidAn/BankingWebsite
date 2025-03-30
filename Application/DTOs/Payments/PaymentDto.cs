using Domain.Enums;

namespace Application.DTOs.Payments;

public class PaymentDto
{
    public string Id { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string? Description { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
