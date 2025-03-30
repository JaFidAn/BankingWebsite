using Application.DTOs.Transactions;
using FluentValidation;

namespace Application.Validators.Transactions;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Transaction type is required")
            .Must(type =>
                type == "Transfer" ||
                type == "Deposit" ||
                type == "Withdraw")
            .WithMessage("Transaction type must be Transfer, Deposit or Withdraw");

        RuleFor(x => x)
            .Must(dto =>
                dto.Type != "Transfer" && dto.Type != "Withdraw" || !string.IsNullOrWhiteSpace(dto.FromAccountId))
            .WithMessage("FromAccountId is required for Transfer and Withdraw transactions");

        RuleFor(x => x)
            .Must(dto =>
                dto.Type != "Transfer" || !string.IsNullOrWhiteSpace(dto.ToAccountId))
            .WithMessage("ToAccountId is required for Transfer transactions");
    }
}
