using SharedKernel.Domain.Entities;

namespace Modules.Notifications.Domain.Notifications.ValueObjects;

public sealed record NotificationId : EntityId
{
    private NotificationId(Guid value) : base(value)
    {
    }

    public static NotificationId New() => new(Guid.NewGuid());
    public static NotificationId From(Guid value) => new(value);
}
