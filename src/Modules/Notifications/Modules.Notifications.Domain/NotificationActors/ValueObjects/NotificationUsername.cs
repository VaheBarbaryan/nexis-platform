using Modules.Notifications.Domain.NotificationActors.Exceptions;

namespace Modules.Notifications.Domain.NotificationActors.ValueObjects;

public sealed record NotificationUsername
{
    public string Value { get; }

    private NotificationUsername(string value) => Value = value;

    public static NotificationUsername From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NotificationUsernameCannotBeEmptyException();
        }

        return new NotificationUsername(value.Trim());
    }
}
