using Application.Repositories.PaymentRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.PaymentRepositories;

public class PaymentReadRepository : ReadRepository<Payment>, IPaymentReadRepository
{
    public PaymentReadRepository(ApplicationDbContext context) : base(context) { }
}
