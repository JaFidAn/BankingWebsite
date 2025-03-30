using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Transaction : BaseEntity
{
    public string FromAccountId { get; set; } = null!;
    public string? ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; } = TransactionType.Transfer;
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Account FromAccount { get; set; } = null!;
    public Account? ToAccount { get; set; }
}
