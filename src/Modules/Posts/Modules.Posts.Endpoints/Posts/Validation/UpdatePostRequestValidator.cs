using FluentValidation;
using Modules.Posts.Endpoints.Posts.Contracts;

namespace Modules.Posts.Endpoints.Posts.Validation;

internal sealed class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(500);
    }
}
