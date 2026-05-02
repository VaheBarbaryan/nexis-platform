Generate an integration event message type for cross-module/external communication. Arguments: `$ARGUMENTS`

Expected format: `<EventMessageName> <Module>`

Example: `StoryPublished Posts`
→ creates `StoryPublishedEventMessage` in `Modules.Posts.IntegrationEvents`

## File to generate

`src/Modules/<Module>/Modules.<Module>.IntegrationEvents/<EventMessageName>EventMessage.cs`

```csharp
using SharedKernel.Domain.Events;

namespace Modules.<Module>.IntegrationEvents;

public sealed record <EventMessageName>EventMessage : IntegrationEvent
{
    public required string Action { get; init; }  // e.g. "created" | "updated" | "deleted"
    // entity-specific fields — use primitive types (Guid, string, int, DateTimeOffset)
    // NO domain value objects here — this crosses module boundaries
}
```

## IntegrationEvent base (reference)

```csharp
public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
    public int SchemaVersion { get; init; } = 1;
    public Guid? CorrelationId { get; init; }
}
```

## Kafka topic

Check `SharedKernel.Infrastructure.Messaging.KafkaTopics` for existing topic constants. If a new topic is needed, add it there:

```csharp
// in KafkaTopics.cs
public const string <ModuleName>EventsV1 = "<module>-events-v1";
```

## Consumer side (if another module needs to consume this event)

If a consuming module is specified, also generate the consumer handler in the consumer's Infrastructure layer:

```csharp
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Modules.<SourceModule>.IntegrationEvents;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.<ConsumerModule>.Infrastructure.Consumers;

internal sealed class <EventMessageName>Consumer : KafkaConsumerBase<<EventMessageName>EventMessage>
{
    public <EventMessageName>Consumer(/* ILogger, IMediator, etc */) { }

    protected override async Task HandleAsync(<EventMessageName>EventMessage message, CancellationToken ct)
    {
        // handle the event
    }
}
```

## Rules
- Integration events use ONLY primitives — Guid, string, int, bool, DateTimeOffset — never domain value objects or enums (serialize as string)
- `required string Action` for CRUD events that share a single topic per entity type (matches PostEventMessage pattern)
- Separate event types (no Action) for semantically distinct events that warrant their own topic
- `SchemaVersion = 1` starts at 1; increment when schema changes breaking consumers
- After generating, remind me to wire the publisher call in the notification handler (`/gen-domain-event` handles this)
