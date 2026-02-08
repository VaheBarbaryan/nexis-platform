using FluentValidation;
using Modules.Users.Endpoints.Users.Contracts;

namespace Modules.Users.Endpoints.Users.Validation;

public class ResendVerificationRequestValidator : AbstractValidator<ResendVerificationRequest>
{
    public ResendVerificationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
