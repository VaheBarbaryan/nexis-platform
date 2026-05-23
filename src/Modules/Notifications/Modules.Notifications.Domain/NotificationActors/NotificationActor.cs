using Modules.Notifications.Domain.NotificationActors.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Notifications.Domain.NotificationActors;

public sealed class NotificationActor : Entity<NotificationActorId>
{
    public NotificationUsername Username { get; private set; } = null!;

    private NotificationActor()
    {
    }

    private NotificationActor(NotificationActorId id, NotificationUsername username)
    {
        Id = id;
        Username = username;
    }

    public static NotificationActor Create(NotificationActorId id, NotificationUsername username)
    {
        return new NotificationActor(id, username);
    }
}
