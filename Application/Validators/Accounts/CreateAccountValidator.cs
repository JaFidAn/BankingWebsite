using Application.DTOs.Accounts;
using FluentValidation;

namespace Application.Validators.Accounts;

public class CreateAccountValidator : AbstractValidator<CreateAccountDto>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.AccountType)
            .NotEmpty().WithMessage("Account type is required")
            .MaximumLength(50).WithMessage("Account type must not exceed 50 characters");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .MaximumLength(10).WithMessage("Currency must not exceed 10 characters");
    }
}
