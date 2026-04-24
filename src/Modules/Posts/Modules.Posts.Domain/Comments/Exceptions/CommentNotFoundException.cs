using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Comments.Exceptions;

public sealed class CommentNotFoundException : NotFoundException
{
    public CommentNotFoundException()
        : base("Comment not found.")
    {
    }

    public CommentNotFoundException(string message)
        : base(message)
    {
    }

    public CommentNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
