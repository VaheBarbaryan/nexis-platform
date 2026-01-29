namespace SharedKernel.Domain.Events;

public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;

    public int SchemaVersion { get; init; } = 1;

    public Guid? CorrelationId { get; init; }
}
