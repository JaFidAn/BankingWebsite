namespace Application.DTOs.Accounts;

public class AccountDto
{
    public string Id { get; set; } = null!;
    public string AccountType { get; set; } = null!;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
