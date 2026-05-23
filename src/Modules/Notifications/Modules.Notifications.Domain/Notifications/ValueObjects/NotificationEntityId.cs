using SharedKernel.Domain.Entities;

namespace Modules.Notifications.Domain.Notifications.ValueObjects;

public sealed record NotificationEntityId : EntityId
{
    private NotificationEntityId(Guid value) : base(value)
    {
    }

    public static NotificationEntityId From(Guid value) => new(value);
}
