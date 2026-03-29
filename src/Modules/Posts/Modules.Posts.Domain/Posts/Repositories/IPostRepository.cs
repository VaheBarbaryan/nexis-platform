using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task<List<Post>> GetPostsAsync(string? cursor, int limit = 10, CancellationToken cancellationToken = default);

    Task<Post?> GetByIdAsync(PostId postId, CancellationToken ct = default);

    void Add(Post post);
}
