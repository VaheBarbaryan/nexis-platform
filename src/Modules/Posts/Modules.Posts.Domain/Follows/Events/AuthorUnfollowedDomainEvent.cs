using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Follows.Events;

public sealed record AuthorUnfollowedDomainEvent(
    Guid FollowId,
    Guid FollowerId,
    Guid FolloweeId
) : DomainEvent;
