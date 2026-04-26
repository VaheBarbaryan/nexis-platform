using Modules.Posts.Domain.Authors.ValueObjects;

namespace Modules.Posts.Domain.Follows.Repositories;

public interface IFollowRepository
{
    Task<Follow?> GetAsync(AuthorId followerId, AuthorId followeeId, CancellationToken ct = default);

    Task<bool> ExistsAsync(AuthorId followerId, AuthorId followeeId, CancellationToken ct = default);

    void Add(Follow follow);

    void Remove(Follow follow);
}
