using Modules.Posts.Domain.Authors;

namespace Modules.Posts.Application.Contracts;

public interface IAuthorService
{
    Task<Author> CreateAsync(Guid userId, string username, CancellationToken ct);
}
