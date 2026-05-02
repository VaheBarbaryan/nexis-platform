using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Domain.Likes.Repositories;

public interface IPostLikeRepository
{
    Task<PostLike?> GetAsync(PostId postId, AuthorId authorId, CancellationToken ct = default);
    Task<long> GetCountAsync(PostId postId, CancellationToken ct = default);
    Task<Dictionary<Guid, long>> GetCountsAsync(IEnumerable<PostId> postIds, CancellationToken ct = default);

    /// <summary>Returns the set of post IDs (from the provided list) that the given author has liked.</summary>
    Task<HashSet<Guid>> GetLikedByUserAsync(IEnumerable<PostId> postIds, AuthorId authorId, CancellationToken ct = default);

    Task RefreshCountsAsync(CancellationToken ct = default);
    void Add(PostLike postLike);
    void Remove(PostLike postLike);
}
