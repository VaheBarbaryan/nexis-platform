using Modules.Notifications.Domain.NotificationActors;

namespace Modules.Notifications.Application.Contracts;

public interface INotificationActorService
{
    Task<NotificationActor> CreateAsync(Guid userId, string username, CancellationToken ct);
}
