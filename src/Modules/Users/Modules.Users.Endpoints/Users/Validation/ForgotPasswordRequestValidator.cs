using FluentValidation;
using Modules.Users.Endpoints.Users.Contracts;

namespace Modules.Users.Endpoints.Users.Validation;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
