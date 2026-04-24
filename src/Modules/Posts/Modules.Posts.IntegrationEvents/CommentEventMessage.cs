using SharedKernel.Domain.Events;

namespace Modules.Posts.IntegrationEvents;

public sealed record CommentEventMessage : IntegrationEvent
{
    public required string Action { get; init; } // "created" | "updated" | "deleted"
    public Guid CommentId { get; init; }
    public Guid PostId { get; init; }
    public Guid AuthorId { get; init; }
    public string? Content { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
