using FluentAssertions;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.Exceptions;
using Modules.Posts.Domain.Follows.Repositories;
using Moq;

namespace Modules.Posts.Application.UnitTests.Services.FollowService;

public sealed class FollowService_UnfollowAsyncTests
{
    private readonly Mock<IFollowRepository> _followRepository = new();
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly Mock<IPostUnitOfWork> _unitOfWork = new();

    private Application.Services.FollowService CreateService() =>
        new(_followRepository.Object, _authorRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task UnfollowAsync_Should_Throw_NotFollowingException_When_Not_Following()
    {
        var service = CreateService();
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _followRepository
            .Setup(x => x.GetAsync(AuthorId.From(followerId), AuthorId.From(followeeId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Follow?)null);

        var act = () => service.UnfollowAsync(followerId, followeeId);

        await act.Should().ThrowAsync<NotFollowingException>();
    }

    [Fact]
    public async Task UnfollowAsync_Should_Not_Commit_When_Not_Following()
    {
        var service = CreateService();
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _followRepository
            .Setup(x => x.GetAsync(AuthorId.From(followerId), AuthorId.From(followeeId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Follow?)null);

        await Assert.ThrowsAsync<NotFollowingException>(() => service.UnfollowAsync(followerId, followeeId));

        _followRepository.Verify(x => x.Remove(It.IsAny<Follow>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UnfollowAsync_Should_Remove_Follow_And_Commit_When_Following()
    {
        var service = CreateService();
        var followerId = AuthorId.New();
        var followeeId = AuthorId.New();
        var follow = Follow.Create(followerId, followeeId);

        _followRepository
            .Setup(x => x.GetAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(follow);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await service.UnfollowAsync(followerId.Value, followeeId.Value);

        _followRepository.Verify(x => x.Remove(follow), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
