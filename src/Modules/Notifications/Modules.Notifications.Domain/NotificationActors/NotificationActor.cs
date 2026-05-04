using Modules.Notifications.Domain.NotificationActors.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Notifications.Domain.NotificationActors;

public sealed class NotificationActor : Entity<NotificationActorId>
{
    public string Username { get; private set; } = null!;

    private NotificationActor()
    {
    }

    private NotificationActor(NotificationActorId id, string username)
    {
        Id = id;
        Username = username;
    }

    public static NotificationActor Create(Guid id, string username)
    {
        return new NotificationActor(new NotificationActorId(id), username);
    }
}
