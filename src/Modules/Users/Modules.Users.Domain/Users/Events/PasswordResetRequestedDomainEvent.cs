using SharedKernel.Domain.Events;

namespace Modules.Users.Domain.Users.Events;

public sealed record PasswordResetRequestedDomainEvent(
    Guid UserId,
    string Username,
    string Email
) : DomainEvent;
