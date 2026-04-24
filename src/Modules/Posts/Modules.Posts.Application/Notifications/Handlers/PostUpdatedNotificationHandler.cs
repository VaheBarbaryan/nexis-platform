using MediatR;
using Modules.Posts.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Posts.Application.Notifications.Handlers;

public sealed class PostUpdatedNotificationHandler : INotificationHandler<PostUpdatedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public PostUpdatedNotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(PostUpdatedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.PostEventsV1,
            new PostEventMessage
            {
                Action = "updated",
                PostId = notification.DomainEvent.PostId,
                AuthorId = notification.DomainEvent.AuthorId,
                Content = notification.DomainEvent.Content,
                OccurredAt = notification.DomainEvent.UpdatedAt
            },
            partitionKey: notification.DomainEvent.PostId.ToString()
        );
    }
}
