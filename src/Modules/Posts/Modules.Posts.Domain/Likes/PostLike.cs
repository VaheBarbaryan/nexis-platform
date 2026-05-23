using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Likes.ValueObjects;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Likes;

public sealed class PostLike : Entity<PostLikeId>
{
    public PostId PostId { get; private set; } = null!;
    public AuthorId AuthorId { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    private PostLike()
    {
    }

    private PostLike(PostId postId, AuthorId authorId)
    {
        Id = PostLikeId.New();
        PostId = postId;
        AuthorId = authorId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static PostLike Create(PostId postId, AuthorId authorId)
        => new(postId, authorId);
}
