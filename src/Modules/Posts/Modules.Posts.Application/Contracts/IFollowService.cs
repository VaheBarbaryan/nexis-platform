using SharedKernel.Application.Pagination;

namespace Modules.Posts.Application.Contracts;

public interface IFollowService
{
    Task FollowAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default);
    Task UnfollowAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default);
    Task<CursorResponse<FollowSummary>> GetFollowersAsync(Guid authorId, string? cursor, int limit, CancellationToken cancellationToken = default);
    Task<CursorResponse<FollowSummary>> GetFollowingAsync(Guid authorId, string? cursor, int limit, CancellationToken cancellationToken = default);
}
