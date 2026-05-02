using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task<List<Post>> GetPostsAsync(string? cursor, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>Returns paginated posts from authors the given user follows, ordered newest-first.</summary>
    Task<List<Post>> GetFeedAsync(AuthorId userId, string? cursor, int limit = 10, CancellationToken ct = default);

    Task<Post?> GetByIdAsync(PostId postId, CancellationToken ct = default);

    void Add(Post post);
}
