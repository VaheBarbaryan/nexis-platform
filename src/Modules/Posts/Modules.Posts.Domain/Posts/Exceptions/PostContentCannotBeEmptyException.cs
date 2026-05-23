using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Posts.Exceptions;

public sealed class PostContentCannotBeEmptyException : DomainValidationException
{
    public PostContentCannotBeEmptyException()
        : base("Post content cannot be empty.")
    {
    }

    public PostContentCannotBeEmptyException(string message)
        : base(message)
    {
    }

    public PostContentCannotBeEmptyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
