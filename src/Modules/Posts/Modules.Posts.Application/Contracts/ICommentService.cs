using SharedKernel.Application.Pagination;

namespace Modules.Posts.Application.Contracts;

public interface ICommentService
{
    Task<CursorResponse<CommentSummary>> GetByPostIdAsync(Guid postId, string? cursor, int limit = 20,
        CancellationToken ct = default);

    Task<CommentSummary> CreateAsync(Guid authorId, Guid postId, string content, CancellationToken ct = default);

    Task<CommentSummary> UpdateAsync(Guid authorId, Guid commentId, string content, CancellationToken ct = default);

    Task DeleteAsync(Guid authorId, Guid commentId, CancellationToken ct = default);
}
