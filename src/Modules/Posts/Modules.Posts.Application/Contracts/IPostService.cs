using Modules.Posts.Domain.Posts;

namespace Modules.Posts.Application.Contracts;

public interface IPostService
{
    Task<Post> CreateAsync(Guid authorId, string content, CancellationToken ct);
}
