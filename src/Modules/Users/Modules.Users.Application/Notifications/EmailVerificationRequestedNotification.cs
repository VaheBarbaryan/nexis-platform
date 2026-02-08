using Modules.Users.Domain.Users.Events;
using SharedKernel.Application.Events;

namespace Modules.Users.Application.Notifications;

public sealed class EmailVerificationRequestedNotification : DomainNotificationBase<EmailVerificationRequestedDomainEvent>
{
    public EmailVerificationRequestedNotification(EmailVerificationRequestedDomainEvent domainEvent, Guid id) : base(
        domainEvent, id)
    {
    }
}
