using MediatR;
using Modules.Posts.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Posts.Application.Notifications.Handlers;

public sealed class CommentUpdatedNotificationHandler : INotificationHandler<CommentUpdatedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public CommentUpdatedNotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(CommentUpdatedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.PostCommentEventsV1,
            new CommentEventMessage
            {
                Action = "updated",
                CommentId = notification.DomainEvent.CommentId,
                PostId = notification.DomainEvent.PostId,
                AuthorId = notification.DomainEvent.AuthorId,
                Content = notification.DomainEvent.Content,
                OccurredAt = notification.DomainEvent.UpdatedAt
            },
            partitionKey: notification.DomainEvent.PostId.ToString()
        );
    }
}
