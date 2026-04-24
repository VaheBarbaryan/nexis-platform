using Modules.Posts.Domain.Posts.Events;
using SharedKernel.Application.Events;

namespace Modules.Posts.Application.Notifications;

public sealed class PostDeletedNotification : DomainNotificationBase<PostDeletedDomainEvent>
{
    public PostDeletedNotification(PostDeletedDomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
