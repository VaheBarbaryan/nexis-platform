using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Posts.Events;

public sealed record PostDeletedDomainEvent(
    Guid PostId,
    Guid AuthorId,
    DateTimeOffset DeletedAt
) : DomainEvent;
