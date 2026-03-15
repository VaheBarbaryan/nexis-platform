using FluentValidation;
using Modules.Posts.Endpoints.Posts.Contracts;

namespace Modules.Posts.Endpoints.Posts.Validation;

internal sealed class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(500);
    }
}
