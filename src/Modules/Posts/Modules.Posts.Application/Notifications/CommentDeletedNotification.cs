using Modules.Posts.Domain.Comments.Events;
using SharedKernel.Application.Events;

namespace Modules.Posts.Application.Notifications;

public sealed class CommentDeletedNotification : DomainNotificationBase<CommentDeletedDomainEvent>
{
    public CommentDeletedNotification(CommentDeletedDomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
