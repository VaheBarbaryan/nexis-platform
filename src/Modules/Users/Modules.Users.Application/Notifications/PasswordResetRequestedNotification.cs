using Modules.Users.Domain.Users.Events;
using SharedKernel.Application.Events;

namespace Modules.Users.Application.Notifications;

public sealed class PasswordResetRequestedNotification : DomainNotificationBase<PasswordResetRequestedDomainEvent>
{
    public PasswordResetRequestedNotification(PasswordResetRequestedDomainEvent domainEvent, Guid id) : base(
        domainEvent, id)
    {
    }
}
