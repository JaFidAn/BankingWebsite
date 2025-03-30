using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Payment : BaseEntity
{
    public string AccountId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "AZN";
    public string? Description { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Account Account { get; set; } = null!;
}
