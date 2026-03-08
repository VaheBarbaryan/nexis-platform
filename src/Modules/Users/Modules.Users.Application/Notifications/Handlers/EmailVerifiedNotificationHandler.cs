using MediatR;
using Modules.Users.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Users.Application.Notifications.Handlers;

public sealed class EmailVerifiedNotificationHandler : INotificationHandler<EmailVerifiedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public EmailVerifiedNotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(EmailVerifiedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        Console.WriteLine($"Email verification notification handler {notification}");

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.UserEventsV1,
            new UserEventMessage
            {
                Action = "created",
                UserId = notification.DomainEvent.UserId,
                Email = notification.DomainEvent.Email,
                Username = notification.DomainEvent.Username
            },
            partitionKey: notification.DomainEvent.UserId.ToString()
        );
    }
}
