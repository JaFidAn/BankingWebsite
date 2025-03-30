using Domain.Entities.Common;

namespace Domain.Entities;

public class Account : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string AccountType { get; set; } = null!;
    public decimal Balance { get; set; } = 0;
    public string Currency { get; set; } = "AZN";
    public bool IsActive { get; set; } = true;

    public AppUser User { get; set; } = null!;
}
