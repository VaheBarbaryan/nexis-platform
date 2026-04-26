using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.Repositories;
using Modules.Posts.Persistence.Contexts;

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

    public void Add(Follow follow)
    {
        _context.Follows.Add(follow);
    }

    public void Remove(Follow follow)
    {
        _context.Follows.Remove(follow);
    }
}
