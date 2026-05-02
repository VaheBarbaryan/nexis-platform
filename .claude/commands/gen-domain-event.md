Generate the full domain event pipeline for this modular monolith. Arguments: `$ARGUMENTS`

Expected format: `<EventName> <Module> <Entity>`

Example: `PostCreated Posts Post` → generates PostCreatedDomainEvent + PostCreatedNotification + PostCreatedNotificationHandler

The notification handler publishes to Kafka. If this event should NOT publish to Kafka, say so explicitly.

## Files to generate

### 1. Domain event — `src/Modules/<Module>/Modules.<Module>.Domain/<Entity>/Events/<EventName>DomainEvent.cs`

```csharp
using SharedKernel.Domain.Events;

namespace Modules.<Module>.Domain.<Entity>.Events;

public sealed record <EventName>DomainEvent(
    // constructor params — typically Id fields (Guid) and relevant data
    // Use primitive types, NOT value objects (Guid not PostId)
) : DomainEvent;
```

### 2. Notification — `src/Modules/<Module>/Modules.<Module>.Application/Notifications/<EventName>Notification.cs`

```csharp
using Modules.<Module>.Domain.<Entity>.Events;
using SharedKernel.Application.Events;

namespace Modules.<Module>.Application.Notifications;

public sealed class <EventName>Notification : DomainNotificationBase<<EventName>DomainEvent>
{
    public <EventName>Notification(<EventName>DomainEvent domainEvent, Guid id) : base(domainEvent, id)
    {
    }
}
```

### 3. Notification handler — `src/Modules/<Module>/Modules.<Module>.Application/Notifications/Handlers/<EventName>NotificationHandler.cs`

Pattern when publishing to Kafka (default):
```csharp
using MediatR;
using Modules.<Module>.IntegrationEvents;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.<Module>.Application.Notifications.Handlers;

public sealed class <EventName>NotificationHandler : INotificationHandler<<EventName>Notification>
{
    private readonly IEventBusPublisher _eventBusPublisher;

    public <EventName>NotificationHandler(IEventBusPublisher eventBusPublisher)
    {
        _eventBusPublisher = eventBusPublisher;
    }

    public async Task Handle(<EventName>Notification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.<TopicConstant>,
            new <IntegrationEventMessage>
            {
                Action = "<action>",   // "created" | "updated" | "deleted"
                // map fields from notification.DomainEvent
            },
            partitionKey: notification.DomainEvent.<PrimaryId>.ToString()
        );
    }
}
```

Pattern when NOT publishing to Kafka:
```csharp
using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace Modules.<Module>.Application.Notifications.Handlers;

[SuppressMessage("Usage", "CA1812", Justification = "Instantiated via DI")]
internal sealed class <EventName>NotificationHandler : INotificationHandler<<EventName>Notification>
{
    public Task Handle(<EventName>Notification notification, CancellationToken cancellationToken)
    {
        // side-effect logic here
        return Task.CompletedTask;
    }
}
```

## Rules
- Domain event params: use Guid for IDs (not value objects), DateTimeOffset for timestamps
- The notification class is the MediatR bridge from outbox → application layer
- Check `KafkaTopics` in `SharedKernel.Infrastructure.Messaging` for existing topic constants
- Check `src/Modules/<Module>/Modules.<Module>.IntegrationEvents/` for existing integration event message types
- After generating files, remind me to also call `RaiseDomainEvent(new <EventName>DomainEvent(...))` from the aggregate method that triggers this event
