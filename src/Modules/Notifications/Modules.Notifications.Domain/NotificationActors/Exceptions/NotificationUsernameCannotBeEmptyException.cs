using SharedKernel.Domain.Exceptions;

namespace Modules.Notifications.Domain.NotificationActors.Exceptions;

public sealed class NotificationUsernameCannotBeEmptyException : DomainValidationException
{
    public NotificationUsernameCannotBeEmptyException()
        : base("Notification username content cannot be empty.")
    {
    }

    public NotificationUsernameCannotBeEmptyException(string message)
        : base(message)
    {
    }

    public NotificationUsernameCannotBeEmptyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
