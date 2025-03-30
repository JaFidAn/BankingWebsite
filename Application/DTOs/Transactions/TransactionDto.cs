namespace Application.DTOs.Transactions;

public class TransactionDto
{
    public string Id { get; set; } = null!;
    public string? FromAccountId { get; set; }
    public string? ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
