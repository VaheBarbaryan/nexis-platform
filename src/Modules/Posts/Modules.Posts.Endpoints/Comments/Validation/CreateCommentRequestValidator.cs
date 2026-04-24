using FluentValidation;
using Modules.Posts.Endpoints.Comments.Contracts;

namespace Modules.Posts.Endpoints.Comments.Validation;

public sealed class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(10000);
    }
}
