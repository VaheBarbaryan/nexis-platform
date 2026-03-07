using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Posts.Events;

public sealed record PostCreatedDomainEvent(
    Guid PostId,
    Guid AuthorId,
    string Content,
    DateTimeOffset CreatedAt
) : DomainEvent;
