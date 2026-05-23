using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Posts.ValueObjects;

public sealed record PostId : EntityId
{
    private PostId(Guid value) : base(value)
    {
    }

    public static PostId New() => new(Guid.NewGuid());
    public static PostId From(Guid value) => new(value);
}
