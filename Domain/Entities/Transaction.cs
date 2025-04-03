using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Transaction : BaseEntity
{
    public string? FromAccountId { get; set; }
    public string? ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; } = TransactionType.Transfer;
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public bool IsSuspicious { get; set; } = false;

    public Account? FromAccount { get; set; }
    public Account? ToAccount { get; set; }
}
