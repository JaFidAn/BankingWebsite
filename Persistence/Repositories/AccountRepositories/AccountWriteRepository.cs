using Application.Repositories.AccountRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.AccountRepositories;

public class AccountWriteRepository : WriteRepository<Account>, IAccountWriteRepository
{
    public AccountWriteRepository(ApplicationDbContext context) : base(context) { }
}
