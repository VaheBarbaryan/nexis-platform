using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Likes.ValueObjects;

public sealed record PostLikeId : EntityId
{
    private PostLikeId(Guid value) : base(value)
    {
    }

    public static PostLikeId New() => new(Guid.NewGuid());
    public static PostLikeId From(Guid value) => new(value);
}
