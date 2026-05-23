using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Authors.ValueObjects;

public sealed record AuthorId : EntityId
{
    private AuthorId(Guid value) : base(value)
    {
    }

    public static AuthorId New() => new(Guid.NewGuid());
    public static AuthorId From(Guid value) => new(value);
}
