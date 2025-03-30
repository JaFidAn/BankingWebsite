namespace Application.DTOs.Accounts;

public class CreateAccountDto
{
    public string AccountType { get; set; } = null!;
    public string Currency { get; set; } = "AZN";
}
