using Application.DTOs.Payments;
using FluentValidation;

namespace Application.Validators.Payments;

public class PaymentRequestValidator : AbstractValidator<PaymentRequestDto>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Stripe token is required");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");
    }
}
