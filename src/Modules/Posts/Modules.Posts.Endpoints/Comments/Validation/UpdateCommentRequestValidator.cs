using FluentValidation;
using Modules.Posts.Endpoints.Comments.Contracts;

namespace Modules.Posts.Endpoints.Comments.Validation;

public sealed class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
