using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.Messaging.Commands;

public sealed record SendEmailIntegrationCommand : IntegrationEvent
{
    public required string Template { get; init; }
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string Language { get; init; }
    public Dictionary<string, string> Variables { get; init; } = null!;
}
