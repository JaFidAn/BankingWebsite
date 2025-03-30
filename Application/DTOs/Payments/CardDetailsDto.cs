namespace Application.DTOs.Payments;

public class CardDetailsDto
{
    public string Number { get; set; } = null!;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Cvv { get; set; } = null!;
}
