using FluentValidation;
using Modules.Users.Endpoints.Users.Contracts;

namespace Modules.Users.Endpoints.Users.Validation;

internal sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

        RuleFor(x => x.NewPasswordConfirm)
            .NotEmpty()
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match");
    }
}
