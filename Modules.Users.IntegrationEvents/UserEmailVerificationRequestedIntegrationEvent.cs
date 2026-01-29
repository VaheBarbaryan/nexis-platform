using System.Diagnostics.CodeAnalysis;
using SharedKernel.Domain.Events;

namespace Modules.Users.IntegrationEvents;

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "Integration event contracts use language-neutral primitives")]
[SuppressMessage(
    "Design",
    "CA1056:URI-like properties should not be strings",
    Justification = "Integration event contracts use language-neutral primitives")]
public sealed record UserEmailVerificationRequestedIntegrationEvent(
    Guid UserId,
    string Email,
    string VerificationUrl) : IntegrationEvent;
