using SharedKernel.Domain.Rules;

namespace Modules.Notifications.Domain.Notifications.Rules;

public sealed class NotificationMustNotBeAlreadyReadRule : IBusinessRule
{
    private readonly bool _isRead;

    public NotificationMustNotBeAlreadyReadRule(bool isRead)
    {
        _isRead = isRead;
    }

    public bool IsBroken() => _isRead;

    public string Message => "Cannot read already read notification.";
}
