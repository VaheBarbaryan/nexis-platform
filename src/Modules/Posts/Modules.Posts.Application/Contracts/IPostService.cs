using Modules.Posts.Domain.Posts;
using SharedKernel.Application.Pagination;

namespace Modules.Posts.Application.Contracts;

public interface IPostService
{
    Task<CursorResponse<PostSummary>> GetPostsAsync(string? cursor, int limit = 10, CancellationToken ct = default);

    Task<Post> GetByIdAsync(Guid postId, CancellationToken ct = default);

    Task<Post> CreateAsync(Guid authorId, string content, CancellationToken ct = default);

    Task<Post> UpdateAsync(Guid authorId, Guid postId, string content, CancellationToken ct = default);

    Task DeleteAsync(Guid authorId, Guid postId, CancellationToken ct = default);
}
