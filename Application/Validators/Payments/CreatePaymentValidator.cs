using Application.DTOs.Payments;
using FluentValidation;

namespace Application.Validators.Payments;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .MaximumLength(10).WithMessage("Currency must not exceed 10 characters");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description cannot exceed 250 characters");
    }
}
