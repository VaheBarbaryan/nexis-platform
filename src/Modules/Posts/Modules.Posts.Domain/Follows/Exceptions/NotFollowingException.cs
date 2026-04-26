using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Follows.Exceptions;

public sealed class NotFollowingException : NotFoundException
{
    public NotFollowingException()
        : base("You are not following the specified author.")
    {
    }

    public NotFollowingException(string message)
        : base(message)
    {
    }

    public NotFollowingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
