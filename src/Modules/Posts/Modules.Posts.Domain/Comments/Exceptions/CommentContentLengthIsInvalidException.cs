using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Comments.Exceptions;

public sealed class CommentContentLengthIsInvalidException : DomainValidationException
{
    public CommentContentLengthIsInvalidException()
        : base("Comment content cannot exceed 10000 characters.")
    {
    }

    public CommentContentLengthIsInvalidException(string message)
        : base(message)
    {
    }

    public CommentContentLengthIsInvalidException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
