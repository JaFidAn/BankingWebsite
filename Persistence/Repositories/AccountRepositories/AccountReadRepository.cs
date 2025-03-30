using Application.Repositories.AccountRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.AccountRepositories;

public class AccountReadRepository : ReadRepository<Account>, IAccountReadRepository
{
    public AccountReadRepository(ApplicationDbContext context) : base(context) { }
}
