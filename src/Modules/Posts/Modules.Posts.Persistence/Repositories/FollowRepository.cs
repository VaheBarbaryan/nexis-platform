using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.Repositories;
using Modules.Posts.Persistence.Contexts;
using SharedKernel.Application.Pagination;

namespace Modules.Posts.Persistence.Repositories;

public sealed class FollowRepository : IFollowRepository
{
    private readonly PostsDbContext _context;

    public FollowRepository(PostsDbContext context)
    {
        _context = context;
    }

    public async Task<Follow?> GetAsync(AuthorId followerId, AuthorId followeeId, CancellationToken ct = default)
    {
        return await _context.Follows.FirstOrDefaultAsync(
            f => f.FollowerId == followerId && f.FolloweeId == followeeId,
            ct);
    }

    public async Task<bool> ExistsAsync(AuthorId followerId, AuthorId followeeId, CancellationToken ct = default)
    {
        return await _context.Follows.AnyAsync(
            f => f.FollowerId == followerId && f.FolloweeId == followeeId,
            ct);
    }

    public async Task<List<Follow>> GetFollowersAsync(
        AuthorId authorId,
        string? cursor,
        int limit,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(authorId);

        var decoded = Cursor.Decode(cursor);
        var authorIdValue = authorId.Value;

        if (decoded is null)
        {
            return await _context.Follows
                .Where(f => f.FolloweeId == authorId)
                .OrderByDescending(f => f.CreatedAt)
                .ThenByDescending(f => f.Id)
                .Take(limit + 1)
                .ToListAsync(ct);
        }

        var lastDate = decoded.Date;
        var lastId = decoded.LastId;

        return await _context.Follows
            .FromSqlInterpolated($"""
                SELECT * FROM posts.follows
                WHERE followee_id = {authorIdValue}
                  AND (created_at, id) < ({lastDate}, {lastId})
                ORDER BY created_at DESC, id DESC
                LIMIT {limit + 1}
                """)
            .ToListAsync(ct);
    }

    public async Task<List<Follow>> GetFollowingAsync(
        AuthorId authorId,
        string? cursor,
        int limit,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(authorId);

        var decoded = Cursor.Decode(cursor);
        var authorIdValue = authorId.Value;

        if (decoded is null)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == authorId)
                .OrderByDescending(f => f.CreatedAt)
                .ThenByDescending(f => f.Id)
                .Take(limit + 1)
                .ToListAsync(ct);
        }

        var lastDate = decoded.Date;
        var lastId = decoded.LastId;

        return await _context.Follows
            .FromSqlInterpolated($"""
                SELECT * FROM posts.follows
                WHERE follower_id = {authorIdValue}
                  AND (created_at, id) < ({lastDate}, {lastId})
                ORDER BY created_at DESC, id DESC
                LIMIT {limit + 1}
                """)
            .ToListAsync(ct);
    }

    public void Add(Follow follow)
    {
        _context.Follows.Add(follow);
    }

    public void Remove(Follow follow)
    {
        _context.Follows.Remove(follow);
    }
}
