namespace Application.DTOs.Transactions;

public class CreateTransactionDto
{
    public string? FromAccountId { get; set; }
    public string? ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
}
