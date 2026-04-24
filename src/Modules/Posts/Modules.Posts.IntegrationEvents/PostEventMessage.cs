using SharedKernel.Domain.Events;

namespace Modules.Posts.IntegrationEvents;

public sealed record PostEventMessage : IntegrationEvent
{
    public required string Action { get; init; } // "created" | "updated" | "deleted"
    public Guid PostId { get; init; }
    public Guid AuthorId { get; init; }
    public string? Content { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
