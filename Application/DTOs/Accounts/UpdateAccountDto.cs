namespace Application.DTOs.Accounts;

public class UpdateAccountDto
{
    public string AccountType { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public bool IsActive { get; set; }
}
