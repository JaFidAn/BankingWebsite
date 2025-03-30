namespace Application.DTOs.Payments;

public class PaymentRequestDto
{
    public string AccountId { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string Token { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = null!;
}
