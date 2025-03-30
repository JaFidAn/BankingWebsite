using Application.Repositories.TransactionRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.TransactionRepositories;

public class TransactionWriteRepository : WriteRepository<Transaction>, ITransactionWriteRepository
{
    public TransactionWriteRepository(ApplicationDbContext context) : base(context)
    {
    }
}
