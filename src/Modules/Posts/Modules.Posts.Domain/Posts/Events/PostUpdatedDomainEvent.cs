using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Posts.Events;

public sealed record PostUpdatedDomainEvent(
    Guid PostId,
    Guid AuthorId,
    string Content,
    DateTimeOffset UpdatedAt
) : DomainEvent;
