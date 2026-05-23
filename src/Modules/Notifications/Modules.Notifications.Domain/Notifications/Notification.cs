using Modules.Notifications.Domain.Notifications.Enums;
using Modules.Notifications.Domain.Notifications.Rules;
using Modules.Notifications.Domain.Notifications.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Notifications.Domain.Notifications;

public sealed class Notification : Entity<NotificationId>
{
    public NotificationUserId RecipientId { get; private set; }
    public NotificationUserId ActorId { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationEntityId EntityId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Required by EF Core for materialization.
    /// Properties are guaranteed to be set before any domain interaction.
    /// </summary>
#pragma warning disable CS8618
    private Notification()
    {
    }
#pragma warning restore CS8618

    private Notification(NotificationUserId recipientId, NotificationUserId actorId, NotificationType type,
        NotificationEntityId entityId)
    {
        CheckRule(new CannotCreateSelfNotificationRule(recipientId, actorId));

        Id = NotificationId.New();
        RecipientId = recipientId;
        ActorId = actorId;
        Type = type;
        EntityId = entityId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Creates a notification. ActorId must differ from RecipientId — no self-notifications.</summary>
    public static Notification Create(NotificationUserId recipientId, NotificationUserId actorId, NotificationType type,
        NotificationEntityId entityId)
    {
        return new Notification(recipientId, actorId, type, entityId);
    }

    public void MarkAsRead()
    {
        CheckRule(new NotificationMustNotBeAlreadyReadRule(IsRead));

        IsRead = true;
    }
}
