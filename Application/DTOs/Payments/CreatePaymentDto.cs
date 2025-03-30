namespace Application.DTOs.Payments;

public class CreatePaymentDto
{
    public string AccountId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "AZN";
    public string? Description { get; set; }
}
