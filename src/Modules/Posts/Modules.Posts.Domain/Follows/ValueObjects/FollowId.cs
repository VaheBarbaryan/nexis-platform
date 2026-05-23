using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Follows.ValueObjects;

public sealed record FollowId : EntityId
{
    private FollowId(Guid value) : base(value)
    {
    }

    public static FollowId New() => new(Guid.NewGuid());
    public static FollowId From(Guid value) => new(value);
}
