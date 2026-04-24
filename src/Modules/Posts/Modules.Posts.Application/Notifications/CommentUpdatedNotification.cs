using Modules.Posts.Domain.Comments.Events;
using SharedKernel.Application.Events;

namespace Modules.Posts.Application.Notifications;

public sealed class CommentUpdatedNotification : DomainNotificationBase<CommentUpdatedDomainEvent>
{
    public CommentUpdatedNotification(CommentUpdatedDomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
