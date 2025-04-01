using Application.DTOs.Users;
using FluentValidation;

namespace Application.Validators.Users;

public class Toggle2FaValidator : AbstractValidator<Toggle2FaDto>
{
    public Toggle2FaValidator()
    {
        RuleFor(x => x.IsEnabled)
            .NotNull().WithMessage("2FA toggle value is required");
    }
}
