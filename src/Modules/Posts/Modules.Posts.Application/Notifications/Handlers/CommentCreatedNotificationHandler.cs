using MediatR;
using Modules.Posts.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Posts.Application.Notifications.Handlers;

public sealed class CommentCreatedNotificationHandler : INotificationHandler<CommentCreatedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public CommentCreatedNotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(CommentCreatedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.PostCommentEventsV1,
            new CommentEventMessage
            {
                Action = "created",
                CommentId = notification.DomainEvent.CommentId,
                PostId = notification.DomainEvent.PostId,
                AuthorId = notification.DomainEvent.AuthorId,
                Content = notification.DomainEvent.Content,
                OccurredAt = notification.DomainEvent.CreatedAt
            },
            partitionKey: notification.DomainEvent.PostId.ToString()
        );
    }
}
