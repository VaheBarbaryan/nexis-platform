using Modules.Posts.Domain.Comments.ValueObjects;
using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Domain.Comments.Repositories;

public interface ICommentRepository
{
    Task<List<Comment>> GetByPostIdAsync(PostId postId, string? cursor, int limit = 20, CancellationToken ct = default);

    Task<Comment?> GetByIdAsync(CommentId commentId, CancellationToken ct = default);

    void Add(Comment comment);
}
