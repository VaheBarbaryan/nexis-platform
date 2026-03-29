using Microsoft.EntityFrameworkCore;
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

    public async Task<Post?> GetByIdAsync(PostId postId, CancellationToken ct = default)
    {
        return await _context.Posts.SingleOrDefaultAsync(x => x.Id == postId, cancellationToken: ct);
    }

    public void Add(Post post)
    {
        _context.Posts.Add(post);
    }
}
