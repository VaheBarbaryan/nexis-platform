using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Posts.Exceptions;

public sealed class PostNotFoundException : NotFoundException
{
    public PostNotFoundException()
        : base("Post not found.")
    {
    }

    public PostNotFoundException(string message)
        : base(message)
    {
    }

    public PostNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
