using Modules.Posts.Domain.Posts.Events;
using SharedKernel.Application.Events;

namespace Modules.Posts.Application.Notifications;

public sealed class PostCreatedNotification : DomainNotificationBase<PostCreatedDomainEvent>
{
    public PostCreatedNotification(PostCreatedDomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
