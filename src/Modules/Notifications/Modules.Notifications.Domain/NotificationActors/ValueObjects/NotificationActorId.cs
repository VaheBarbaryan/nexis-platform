using SharedKernel.Domain.Entities;

namespace Modules.Notifications.Domain.NotificationActors.ValueObjects;

public sealed record NotificationActorId : EntityId
{
    private NotificationActorId(Guid value) : base(value)
    {
    }

    public static NotificationActorId New() => new(Guid.NewGuid());
    public static NotificationActorId From(Guid value) => new(value);
}
