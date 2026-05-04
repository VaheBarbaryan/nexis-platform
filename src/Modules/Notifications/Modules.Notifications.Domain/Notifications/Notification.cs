using Modules.Notifications.Domain.Notifications.Enums;
using Modules.Notifications.Domain.Notifications.Rules;
using Modules.Notifications.Domain.Notifications.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Notifications.Domain.Notifications;

public sealed class Notification : AggregateRoot<NotificationId>
{
    public Guid RecipientId { get; private set; }
    public Guid ActorId { get; private set; }
    public NotificationType Type { get; private set; }
    public Guid EntityId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Notification()
    {
    }

    private Notification(NotificationId id, Guid recipientId, Guid actorId, NotificationType type, Guid entityId)
    {
        CheckRule(new CannotCreateSelfNotificationRule(recipientId, actorId));

        Id = id;
        RecipientId = recipientId;
        ActorId = actorId;
        Type = type;
        EntityId = entityId;
        IsRead = false;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Creates a notification. ActorId must differ from RecipientId — no self-notifications.</summary>
    public static Notification Create(Guid recipientId, Guid actorId, NotificationType type, Guid entityId)
    {
        return new Notification(new NotificationId(Guid.NewGuid()), recipientId, actorId, type, entityId);
    }

    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
    }
}
