using Modules.Notifications.Domain.NotificationActors.ValueObjects;

namespace Modules.Notifications.Domain.NotificationActors.Repositories;

public interface INotificationActorRepository
{
    Task<bool> ExistsAsync(NotificationActorId id, CancellationToken ct = default);
    void Add(NotificationActor notificationActor);
}
