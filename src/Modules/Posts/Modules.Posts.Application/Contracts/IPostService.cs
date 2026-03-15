using Modules.Posts.Domain.Posts;

namespace Modules.Posts.Application.Contracts;

public interface IPostService
{
    Task<Post> GetByIdAsync(Guid postId, CancellationToken ct = default);

    Task<Post> CreateAsync(Guid authorId, string content, CancellationToken ct = default);

    Task<Post> UpdateAsync(Guid authorId, Guid postId, string content, CancellationToken ct = default);

    Task DeleteAsync(Guid authorId, Guid postId, CancellationToken ct = default);
}
