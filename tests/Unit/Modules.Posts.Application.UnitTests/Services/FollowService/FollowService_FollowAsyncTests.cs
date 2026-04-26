using FluentAssertions;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.Exceptions;
using Modules.Posts.Domain.Follows.Repositories;
using Moq;

namespace Modules.Posts.Application.UnitTests.Services.FollowService;

public sealed class FollowService_FollowAsyncTests
{
    private readonly Mock<IFollowRepository> _followRepository = new();
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly Mock<IPostUnitOfWork> _unitOfWork = new();

    private Application.Services.FollowService CreateService() =>
        new(_followRepository.Object, _authorRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task FollowAsync_Should_Throw_AlreadyFollowException_When_Already_Following()
    {
        var service = CreateService();
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _followRepository
            .Setup(x => x.ExistsAsync(new AuthorId(followerId), new AuthorId(followeeId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => service.FollowAsync(followerId, followeeId);

        await act.Should().ThrowAsync<AlreadyFollowException>();
    }

    [Fact]
    public async Task FollowAsync_Should_Not_Commit_When_Already_Following()
    {
        var service = CreateService();
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _followRepository
            .Setup(x => x.ExistsAsync(new AuthorId(followerId), new AuthorId(followeeId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<AlreadyFollowException>(() => service.FollowAsync(followerId, followeeId));

        _followRepository.Verify(x => x.Add(It.IsAny<Follow>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task FollowAsync_Should_Add_Follow_And_Commit_When_Not_Yet_Following()
    {
        var service = CreateService();
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        _followRepository
            .Setup(x => x.ExistsAsync(new AuthorId(followerId), new AuthorId(followeeId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await service.FollowAsync(followerId, followeeId);

        _followRepository.Verify(
            x => x.Add(It.Is<Follow>(f =>
                f.FollowerId == new AuthorId(followerId) &&
                f.FolloweeId == new AuthorId(followeeId))),
            Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
