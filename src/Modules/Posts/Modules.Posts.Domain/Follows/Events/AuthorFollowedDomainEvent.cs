using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Follows.Events;

public sealed record AuthorFollowedDomainEvent(
    Guid FollowId,
    Guid FollowerId,
    Guid FolloweeId,
    DateTimeOffset CreatedAt
) : DomainEvent;
