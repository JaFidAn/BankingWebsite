using Application.Core;
using Application.DTOs.Payments;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Contexts;
using Stripe;

namespace Infrastructure.Services;

public class StripePaymentService : IStripePaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public StripePaymentService(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;

        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<Result<PaymentResponseDto>> ProcessPaymentAsync(PaymentRequestDto dto, string userId)
    {
        try
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(x => x.Id == dto.AccountId && x.UserId == userId && !x.IsDeleted);

            if (account == null)
                return Result<PaymentResponseDto>.Failure("Account not found or not owned by user", 404);

            var amountInCents = (long)(dto.Amount * 100);

            var chargeOptions = new ChargeCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                Description = dto.Description,
                Source = dto.Token
            };

            var chargeService = new ChargeService();
            var charge = await chargeService.CreateAsync(chargeOptions);

            var payment = new Payment
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Currency = "AZN",
                Description = dto.Description,
                TransactionId = charge.Id,
                PaymentMethod = dto.PaymentMethod,
                Status = charge.Status == "succeeded" ? PaymentStatus.Success : PaymentStatus.Failed
            };

            await _context.Payments.AddAsync(payment);

            if (payment.Status == PaymentStatus.Success)
            {
                account.Balance += dto.Amount;
                _context.Accounts.Update(account);
            }

            var transaction = new Transaction
            {
                FromAccountId = null,
                ToAccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = TransactionType.Deposit,
                Status = payment.Status == PaymentStatus.Success ? TransactionStatus.Success : TransactionStatus.Failed,
                Description = $"Stripe Payment: {dto.Description}"
            };

            await _context.Transactions.AddAsync(transaction);

            await _context.SaveChangesAsync();

            var response = new PaymentResponseDto
            {
                TransactionId = payment.TransactionId!,
                Status = payment.Status.ToString()
            };

            return Result<PaymentResponseDto>.Success(response, "Payment processed successfully");
        }
        catch (Exception ex)
        {
            return Result<PaymentResponseDto>.Failure($"Payment failed: {ex.Message}", 500);
        }
    }
}
