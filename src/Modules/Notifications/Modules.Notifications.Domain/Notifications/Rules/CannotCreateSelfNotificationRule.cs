using SharedKernel.Domain.Rules;

namespace Modules.Notifications.Domain.Notifications.Rules;

public sealed class CannotCreateSelfNotificationRule : IBusinessRule
{
    private readonly Guid _recipientId;
    private readonly Guid _actorId;

    public CannotCreateSelfNotificationRule(Guid recipientId, Guid actorId)
    {
        _recipientId = recipientId;
        _actorId = actorId;
    }

    public bool IsBroken() => _recipientId == _actorId;

    public string Message => "Cannot create self notification";
}
