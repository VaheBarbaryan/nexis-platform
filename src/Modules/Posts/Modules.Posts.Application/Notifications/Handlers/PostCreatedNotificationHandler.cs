using MediatR;
using Modules.Posts.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Posts.Application.Notifications.Handlers;

public sealed class PostCreatedNotificationHandler : INotificationHandler<PostCreatedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public PostCreatedNotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(PostCreatedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.PostEventsV1,
            new PostEventMessage
            {
                Action = "created",
                PostId = notification.DomainEvent.PostId,
                AuthorId = notification.DomainEvent.AuthorId,
                Content = notification.DomainEvent.Content,
                OccurredAt = notification.DomainEvent.CreatedAt
            },
            partitionKey: notification.DomainEvent.PostId.ToString()
        );
    }
}
