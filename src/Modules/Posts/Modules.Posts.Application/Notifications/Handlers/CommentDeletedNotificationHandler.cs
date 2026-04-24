using MediatR;
using Modules.Posts.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Posts.Application.Notifications.Handlers;

public sealed class CommentDeletedNotificationHandler : INotificationHandler<CommentDeletedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public CommentDeletedNotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(CommentDeletedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.PostCommentEventsV1,
            new CommentEventMessage
            {
                Action = "deleted",
                CommentId = notification.DomainEvent.CommentId,
                PostId = notification.DomainEvent.PostId,
                AuthorId = notification.DomainEvent.AuthorId,
                Content = null,
                OccurredAt = notification.DomainEvent.DeletedAt
            },
            partitionKey: notification.DomainEvent.PostId.ToString()
        );
    }
}
