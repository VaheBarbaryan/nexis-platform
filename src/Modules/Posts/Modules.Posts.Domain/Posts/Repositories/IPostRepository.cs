using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(PostId postId, CancellationToken ct = default);

    void Add(Post post);
}
