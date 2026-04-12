using FluentAssertions;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Likes;
using Modules.Posts.Domain.Likes.Repositories;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Exceptions;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using Moq;

namespace Modules.Posts.Application.UnitTests.Services.PostService;

public sealed class PostService_LikeAsyncTests
{
    private readonly Mock<IPostRepository> _postRepository = new();
    private readonly Mock<IPostLikeRepository> _postLikeRepository = new();
    private readonly Mock<IPostUnitOfWork> _unitOfWork = new();

    private Application.Services.PostService CreateService()
        => new(_postRepository.Object, _postLikeRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task LikeAsync_Should_Throw_When_Post_Not_Found()
    {
        // Arrange
        var service = CreateService();
        var postId = Guid.NewGuid();

        _postRepository
            .Setup(x => x.GetByIdAsync(new PostId(postId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        // Act
        var act = () => service.LikeAsync(Guid.NewGuid(), postId);

        // Assert
        await act.Should().ThrowAsync<PostNotFoundException>();
    }

    [Fact]
    public async Task LikeAsync_Should_Do_Nothing_When_Already_Liked()
    {
        // Arrange
        var service = CreateService();
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        var existingLike = PostLike.Create(post.Id, authorId);

        _postRepository
            .Setup(x => x.GetByIdAsync(post.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _postLikeRepository
            .Setup(x => x.GetAsync(post.Id, authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLike);

        // Act
        await service.LikeAsync(authorId.Value, post.Id.Value);

        // Assert
        _postLikeRepository.Verify(x => x.Add(It.IsAny<PostLike>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _postLikeRepository.Verify(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LikeAsync_Should_Add_Like_And_Commit_When_Not_Yet_Liked()
    {
        // Arrange
        var service = CreateService();
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");

        _postRepository
            .Setup(x => x.GetByIdAsync(post.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _postLikeRepository
            .Setup(x => x.GetAsync(post.Id, authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PostLike?)null);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _postLikeRepository
            .Setup(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.LikeAsync(authorId.Value, post.Id.Value);

        // Assert
        _postLikeRepository.Verify(
            x => x.Add(It.Is<PostLike>(l => l.PostId == post.Id && l.AuthorId == authorId)),
            Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _postLikeRepository.Verify(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
