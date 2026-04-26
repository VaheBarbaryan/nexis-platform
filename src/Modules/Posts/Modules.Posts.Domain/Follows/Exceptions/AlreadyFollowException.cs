using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Follows.Exceptions;

public sealed class AlreadyFollowException : ConflictException
{
    public AlreadyFollowException()
        : base("You already follow the specified author.")
    {
    }

    public AlreadyFollowException(string message)
        : base(message)
    {
    }

    public AlreadyFollowException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
