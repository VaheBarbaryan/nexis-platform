using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows.Events;
using Modules.Posts.Domain.Follows.Rules;
using Modules.Posts.Domain.Follows.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Posts.Domain.Follows;

public sealed class Follow : AggregateRoot<FollowId>
{
    public AuthorId FollowerId { get; private set; } = null!;
    public AuthorId FolloweeId { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    private Follow()
    {
    }

    private Follow(AuthorId followerId, AuthorId followeeId)
    {
        CheckRule(new CannotFollowYourselfRule(followerId, followeeId));

        Id = new FollowId(Guid.NewGuid());
        FollowerId = followerId;
        FolloweeId = followeeId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Follow Create(AuthorId followerId, AuthorId followeeId)
    {
        var follow = new Follow(followerId, followeeId);

        follow.RaiseDomainEvent(
            new AuthorFollowedDomainEvent(
                follow.Id.Value,
                follow.FollowerId.Value,
                follow.FolloweeId.Value,
                follow.CreatedAt
            )
        );

        return follow;
    }

    public void Unfollow()
    {
        RaiseDomainEvent(new AuthorUnfollowedDomainEvent(
            Id.Value,
            FollowerId.Value,
            FolloweeId.Value
        ));
    }
}
