using Modules.Posts.Domain.Posts.Events;
using SharedKernel.Application.Events;

namespace Modules.Posts.Application.Notifications;

public sealed class PostUpdatedNotification : DomainNotificationBase<PostUpdatedDomainEvent>
{
    public PostUpdatedNotification(PostUpdatedDomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
