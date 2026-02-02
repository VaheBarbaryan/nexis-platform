using SharedKernel.Domain.Events;

namespace Modules.Users.IntegrationEvents;

/// <summary>
/// Represents the user created integration event.
/// </summary>
/// <param name="UserId"></param>
/// <param name="Email"></param>
/// <param name="UserName"></param>
public sealed record UserCreatedIntegrationEvent(
    Guid UserId,
    string Email,
    string UserName) : IntegrationEvent;
