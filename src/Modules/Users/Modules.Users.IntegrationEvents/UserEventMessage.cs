using SharedKernel.Domain.Events;

namespace Modules.Users.IntegrationEvents;

public sealed record UserEventMessage : IntegrationEvent
{
    public required string Action { get; init; } // "created" | "edited" | "deleted"
    public Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
}
