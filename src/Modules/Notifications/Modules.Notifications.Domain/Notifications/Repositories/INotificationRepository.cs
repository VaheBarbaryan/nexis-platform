using Modules.Notifications.Domain.Notifications.ValueObjects;

namespace Modules.Notifications.Domain.Notifications.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetByRecipientAsync(NotificationUserId recipientId, string? cursor, int limit = 20,
        CancellationToken ct = default);

    Task<Notification?> GetByIdAsync(NotificationId id, CancellationToken ct = default);

    void Add(Notification notification);
}
