using Microsoft.EntityFrameworkCore;
using Modules.Notifications.Domain.Notifications;
using Modules.Notifications.Domain.Notifications.Repositories;
using Modules.Notifications.Domain.Notifications.ValueObjects;
using Modules.Notifications.Persistence.Contexts;
using SharedKernel.Application.Pagination;

namespace Modules.Notifications.Persistence.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationRepository(NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetByRecipientAsync(
        Guid recipientId,
        string? cursor,
        int limit = 20,
        CancellationToken ct = default)
    {
        var decodedCursor = Cursor.Decode(cursor);

        if (decodedCursor is null)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == recipientId)
                .OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.Id)
                .Take(limit + 1)
                .ToListAsync(ct);
        }

        var lastDate = decodedCursor.Date;
        var lastId = decodedCursor.LastId;

        return await _context.Notifications
            .FromSqlInterpolated($"""
                SELECT * FROM notifications.notifications
                WHERE recipient_id = {recipientId}
                  AND (created_at, id) < ({lastDate}, {lastId})
                ORDER BY created_at DESC, id DESC
                LIMIT {limit + 1}
                """)
            .ToListAsync(ct);
    }

    public async Task<Notification?> GetByIdAsync(NotificationId id, CancellationToken ct = default)
    {
        return await _context.Notifications.SingleOrDefaultAsync(n => n.Id == id, ct);
    }

    public void Add(Notification notification)
    {
        _context.Notifications.Add(notification);
    }
}
