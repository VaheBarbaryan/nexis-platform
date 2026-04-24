using Modules.Posts.Domain.Comments.Events;
using SharedKernel.Application.Events;

namespace Modules.Posts.Application.Notifications;

public sealed class CommentCreatedNotification : DomainNotificationBase<CommentCreatedDomainEvent>
{
    public CommentCreatedNotification(CommentCreatedDomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
