using Microsoft.EntityFrameworkCore;
using Modules.Notifications.Domain.NotificationActors;
using Modules.Notifications.Domain.NotificationActors.Repositories;
using Modules.Notifications.Domain.NotificationActors.ValueObjects;
using Modules.Notifications.Persistence.Contexts;

namespace Modules.Notifications.Persistence.Repositories;

public sealed class NotificationActorRepository : INotificationActorRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationActorRepository(NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(NotificationActorId id, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.NotificationActors.AnyAsync(a => a.Id == id, ct);
    }

    public void Add(NotificationActor notificationActor)
    {
        _context.NotificationActors.Add(notificationActor);
    }
}
