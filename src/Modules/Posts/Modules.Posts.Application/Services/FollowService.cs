using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.Exceptions;
using Modules.Posts.Domain.Follows.Repositories;

namespace Modules.Posts.Application.Services;

public sealed class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IPostUnitOfWork _unitOfWork;

    public FollowService(IFollowRepository followRepository, IPostUnitOfWork unitOfWork)
    {
        _followRepository = followRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task FollowAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default)
    {
        var followerIdVo = new AuthorId(followerId);
        var followeeIdVo = new AuthorId(followeeId);

        var exists =
            await _followRepository.ExistsAsync(followerIdVo, followeeIdVo, cancellationToken);

        if (exists)
        {
            throw new AlreadyFollowException();
        }

        var follow = Follow.Create(followerIdVo, followeeIdVo);
        _followRepository.Add(follow);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task UnfollowAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default)
    {
        var follow =
            await _followRepository.GetAsync(new AuthorId(followerId), new AuthorId(followeeId), cancellationToken);

        if (follow is null)
        {
            throw new NotFollowingException();
        }

        follow.Unfollow();
        _followRepository.Remove(follow);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
