using Modules.Users.Domain.Users.Events;
using SharedKernel.Application.Events;

namespace Modules.Users.Application.Notifications;

public sealed class UserCreatedNotification : DomainNotificationBase<UserCreatedDomainEvent>
{
    public UserCreatedNotification(UserCreatedDomainEvent domainEvent, Guid id)
        : base(domainEvent, id)
    {
    }
}
