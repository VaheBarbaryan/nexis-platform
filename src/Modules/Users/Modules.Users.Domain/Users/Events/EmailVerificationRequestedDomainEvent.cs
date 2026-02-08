using SharedKernel.Domain.Events;

namespace Modules.Users.Domain.Users.Events;

public sealed record EmailVerificationRequestedDomainEvent(
    Guid UserId,
    string Username,
    string Email
) : DomainEvent;
