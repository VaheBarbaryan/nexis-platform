using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Posts.Exceptions;

public sealed class PostContentLengthIsInvalidException : DomainValidationException
{
    public PostContentLengthIsInvalidException()
        : base("Post content cannot exceed 500 characters.")
    {
    }

    public PostContentLengthIsInvalidException(string message)
        : base(message)
    {
    }

    public PostContentLengthIsInvalidException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
