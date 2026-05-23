using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Comments.ValueObjects;

public sealed record CommentId : EntityId
{
    private CommentId(Guid value) : base(value)
    {
    }

    public static CommentId New() => new(Guid.NewGuid());
    public static CommentId From(Guid value) => new(value);
}
