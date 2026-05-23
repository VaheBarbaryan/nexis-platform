using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Comments.Exceptions;

public sealed class CommentCannotBeEmptyException : DomainValidationException
{
    public CommentCannotBeEmptyException()
        : base("Comment content cannot be empty.")
    {
    }

    public CommentCannotBeEmptyException(string message)
        : base(message)
    {
    }

    public CommentCannotBeEmptyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
