using Application.Repositories.TransactionRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.TransactionRepositories;

public class TransactionReadRepository : ReadRepository<Transaction>, ITransactionReadRepository
{
    public TransactionReadRepository(ApplicationDbContext context) : base(context)
    {
    }
}
