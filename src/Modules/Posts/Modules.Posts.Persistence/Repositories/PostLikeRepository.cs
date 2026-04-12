using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Likes;
using Modules.Posts.Domain.Likes.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using Modules.Posts.Persistence.Contexts;

namespace Modules.Posts.Persistence.Repositories;

public sealed class PostLikeRepository : IPostLikeRepository
{
    private readonly PostsDbContext _context;

    public PostLikeRepository(PostsDbContext context)
    {
        _context = context;
    }


    public async Task<PostLike?> GetAsync(PostId postId, AuthorId authorId, CancellationToken ct = default)
    {
        return await _context.PostLikes
            .SingleOrDefaultAsync(x => x.PostId == postId && x.AuthorId == authorId, ct);
    }

    public async Task<long> GetCountAsync(PostId postId, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(postId);

        return await _context.Database
            .SqlQuery<long>($"""
                             SELECT count AS "Value" FROM posts.post_like_counts
                             WHERE post_id = {postId.Value}
                             """)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<Dictionary<Guid, long>> GetCountsAsync(
        IEnumerable<PostId> postIds,
        CancellationToken ct = default)
    {
        var ids = postIds.Select(p => p.Value).ToArray();

        if (ids.Length == 0) return [];

        return await _context.Database
            .SqlQuery<PostLikeCountRow>($"""
                                         SELECT post_id, count
                                         FROM posts.post_like_counts
                                         WHERE post_id = ANY({ids})
                                         """)
            .ToDictionaryAsync(r => r.post_id, r => r.count, ct);
    }

    public async Task RefreshCountsAsync(CancellationToken ct = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "REFRESH MATERIALIZED VIEW CONCURRENTLY posts.post_like_counts", ct);
    }

    public void Add(PostLike postLike) => _context.PostLikes.Add(postLike);

    public void Remove(PostLike postLike) => _context.PostLikes.Remove(postLike);

    private sealed record PostLikeCountRow(Guid post_id, long count);
}
