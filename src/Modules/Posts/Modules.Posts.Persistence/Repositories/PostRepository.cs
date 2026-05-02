using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using Modules.Posts.Persistence.Contexts;
using SharedKernel.Application.Pagination;

namespace Modules.Posts.Persistence.Repositories;

public sealed class PostRepository : IPostRepository
{
    private readonly PostsDbContext _context;

    public PostRepository(PostsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetPostsAsync(
        string? cursor,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var decodedCursor = Cursor.Decode(cursor);

        if (decodedCursor is null)
        {
            return await _context.Posts
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Take(limit + 1)
                .ToListAsync(cancellationToken);
        }

        var lastDate = decodedCursor.Date;
        var lastId = decodedCursor.LastId;

        // NOTE: Global query filter (deleted_at IS NULL) does not apply to FromSqlInterpolated,
        // so soft delete filter is applied manually here.
        return await _context.Posts
            .FromSqlInterpolated($"""
                                  SELECT * FROM posts.posts
                                  WHERE deleted_at IS NULL
                                    AND (created_at, id) < ({lastDate}, {lastId})
                                  ORDER BY created_at DESC, id DESC
                                  LIMIT {limit + 1}
                                  """)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetFeedAsync(
        AuthorId userId,
        string? cursor,
        int limit = 10,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var userIdValue = userId.Value;
        var decodedCursor = Cursor.Decode(cursor);

        if (decodedCursor is null)
        {
            return await _context.Posts
                .FromSqlInterpolated($"""
                                      SELECT p.* FROM posts.posts p
                                      INNER JOIN posts.follows f ON p.author_id = f.followee_id
                                      WHERE p.deleted_at IS NULL
                                        AND f.follower_id = {userIdValue}
                                      ORDER BY p.created_at DESC, p.id DESC
                                      LIMIT {limit + 1}
                                      """)
                .ToListAsync(ct);
        }

        var lastDate = decodedCursor.Date;
        var lastId = decodedCursor.LastId;

        return await _context.Posts
            .FromSqlInterpolated($"""
                                  SELECT p.* FROM posts.posts p
                                  INNER JOIN posts.follows f ON p.author_id = f.followee_id
                                  WHERE p.deleted_at IS NULL
                                    AND f.follower_id = {userIdValue}
                                    AND (p.created_at, p.id) < ({lastDate}, {lastId})
                                  ORDER BY p.created_at DESC, p.id DESC
                                  LIMIT {limit + 1}
                                  """)
            .ToListAsync(ct);
    }

    public async Task<Post?> GetByIdAsync(PostId postId, CancellationToken ct = default)
    {
        return await _context.Posts.SingleOrDefaultAsync(x => x.Id == postId, cancellationToken: ct);
    }

    public void Add(Post post)
    {
        _context.Posts.Add(post);
    }
}
