using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Comments;
using Modules.Posts.Domain.Comments.Repositories;
using Modules.Posts.Domain.Comments.ValueObjects;
using Modules.Posts.Domain.Posts.ValueObjects;
using Modules.Posts.Persistence.Contexts;
using SharedKernel.Application.Pagination;

namespace Modules.Posts.Persistence.Repositories;

public sealed class CommentRepository : ICommentRepository
{
    private readonly PostsDbContext _context;

    public CommentRepository(PostsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetByPostIdAsync(
        PostId postId,
        string? cursor,
        int limit = 20,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(postId);

        var decodedCursor = Cursor.Decode(cursor);

        if (decodedCursor is null)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.Id)
                .Take(limit + 1)
                .ToListAsync(ct);
        }

        var lastDate = decodedCursor.Date;
        var lastId = decodedCursor.LastId;
        var postIdValue = postId.Value;

        // NOTE: Global query filter (deleted_at IS NULL) does not apply to FromSqlInterpolated,
        // so soft delete filter is applied manually here.
        return await _context.Comments
            .FromSqlInterpolated($"""
                                  SELECT * FROM posts.comments
                                  WHERE deleted_at IS NULL
                                    AND post_id = {postIdValue}
                                    AND (created_at, id) < ({lastDate}, {lastId})
                                  ORDER BY created_at DESC, id DESC
                                  LIMIT {limit + 1}
                                  """)
            .ToListAsync(ct);
    }

    public async Task<Comment?> GetByIdAsync(CommentId commentId, CancellationToken ct = default)
    {
        return await _context.Comments.SingleOrDefaultAsync(c => c.Id == commentId, ct);
    }

    public void Add(Comment comment)
    {
        _context.Comments.Add(comment);
    }
}
