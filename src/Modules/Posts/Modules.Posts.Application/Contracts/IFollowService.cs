namespace Modules.Posts.Application.Contracts;

public interface IFollowService
{
    Task FollowAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default);
    Task UnfollowAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default);
}
