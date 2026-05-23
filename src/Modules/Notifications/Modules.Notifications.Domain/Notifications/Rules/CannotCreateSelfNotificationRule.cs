using Modules.Notifications.Domain.Notifications.ValueObjects;
using SharedKernel.Domain.Rules;

namespace Modules.Notifications.Domain.Notifications.Rules;

public sealed class CannotCreateSelfNotificationRule : IBusinessRule
{
    private readonly NotificationUserId _recipientId;
    private readonly NotificationUserId _actorId;

    public CannotCreateSelfNotificationRule(NotificationUserId recipientId, NotificationUserId actorId)
    {
        _recipientId = recipientId;
        _actorId = actorId;
    }

    public bool IsBroken() => _recipientId == _actorId;

    public string Message => "Cannot create self notification";
}
