using Modules.Users.Domain.Users.Events;
using SharedKernel.Application.Events;

namespace Modules.Users.Application.Notifications;

public class EmailVerifiedNotification : DomainNotificationBase<UserEmailVerifiedDomainEvent>
{
    public EmailVerifiedNotification(UserEmailVerifiedDomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
