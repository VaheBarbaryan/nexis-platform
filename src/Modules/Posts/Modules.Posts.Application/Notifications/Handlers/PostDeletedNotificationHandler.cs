using MediatR;
using Modules.Posts.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Posts.Application.Notifications.Handlers;

public sealed class PostDeletedNotificationHandler : INotificationHandler<PostDeletedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public PostDeletedNotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(PostDeletedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.PostEventsV1,
            new PostEventMessage
            {
                Action = "deleted",
                PostId = notification.DomainEvent.PostId,
                AuthorId = notification.DomainEvent.AuthorId,
                OccurredAt = notification.DomainEvent.DeletedAt
            },
            partitionKey: notification.DomainEvent.PostId.ToString()
        );
    }
}
