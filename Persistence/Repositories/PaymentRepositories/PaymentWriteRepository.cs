using Application.Repositories.PaymentRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.PaymentRepositories;

public class PaymentWriteRepository : WriteRepository<Payment>, IPaymentWriteRepository
{
    public PaymentWriteRepository(ApplicationDbContext context) : base(context) { }
}
