using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.Exceptions;
using Modules.Posts.Domain.Follows.Repositories;
using SharedKernel.Application.Pagination;

namespace Modules.Posts.Application.Services;

public sealed class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IPostUnitOfWork _unitOfWork;

    public FollowService(
        IFollowRepository followRepository,
        IAuthorRepository authorRepository,
        IPostUnitOfWork unitOfWork)
    {
        _followRepository = followRepository;
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task FollowAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default)
    {
        var followerIdVo = AuthorId.From(followerId);
        var followeeIdVo = AuthorId.From(followeeId);

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
            await _followRepository.GetAsync(AuthorId.From(followerId), AuthorId.From(followeeId), cancellationToken);

        if (follow is null)
        {
            throw new NotFollowingException();
        }

        follow.Unfollow();
        _followRepository.Remove(follow);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<CursorResponse<FollowSummary>> GetFollowersAsync(
        Guid authorId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var follows = await _followRepository.GetFollowersAsync(
            AuthorId.From(authorId), cursor, limit, cancellationToken);

        var authorIds = follows.Select(f => f.FollowerId).Distinct();
        var authors = await _authorRepository.GetByIdsAsync(authorIds, cancellationToken);

        return CursorResponse.From(
            follows,
            limit,
            f =>
            {
                authors.TryGetValue(f.FollowerId, out var author);
                return new FollowSummary(f.FollowerId.Value, author?.Username.Value ?? string.Empty, f.CreatedAt);
            },
            f => new Cursor(f.CreatedAt, f.Id.Value).Encode());
    }

    public async Task<CursorResponse<FollowSummary>> GetFollowingAsync(
        Guid authorId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var follows = await _followRepository.GetFollowingAsync(
            AuthorId.From(authorId), cursor, limit, cancellationToken);

        var authorIds = follows.Select(f => f.FolloweeId).Distinct();
        var authors = await _authorRepository.GetByIdsAsync(authorIds, cancellationToken);

        return CursorResponse.From(
            follows,
            limit,
            f =>
            {
                authors.TryGetValue(f.FolloweeId, out var author);
                return new FollowSummary(f.FolloweeId.Value, author?.Username.Value ?? string.Empty, f.CreatedAt);
            },
            f => new Cursor(f.CreatedAt, f.Id.Value).Encode());
    }
}
