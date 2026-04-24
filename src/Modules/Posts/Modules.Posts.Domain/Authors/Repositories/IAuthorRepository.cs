using Modules.Posts.Domain.Authors.ValueObjects;

namespace Modules.Posts.Domain.Authors.Repositories;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(AuthorId authorId, CancellationToken ct = default);

    Task<Dictionary<AuthorId, Author>> GetByIdsAsync(IEnumerable<AuthorId> authorIds, CancellationToken ct = default);

    void Add(Author author);
}
