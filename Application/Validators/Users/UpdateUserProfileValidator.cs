using Application.DTOs.Users;
using FluentValidation;

namespace Application.Validators.Users;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.NewEmail)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrEmpty(x.NewEmail));

        RuleFor(x => x.NewPhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.NewPhoneNumber));
    }
}
