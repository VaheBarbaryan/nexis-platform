using FluentAssertions;
using Modules.Posts.Application.Services;
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

public sealed class PostService_UnlikeAsyncTests
{
    private readonly Mock<IPostRepository> _postRepository = new();
    private readonly Mock<IPostLikeRepository> _postLikeRepository = new();
    private readonly Mock<IPostUnitOfWork> _unitOfWork = new();

    private Application.Services.PostService CreateService()
        => new(_postRepository.Object, _postLikeRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task UnlikeAsync_Should_Throw_When_Post_Not_Found()
    {
        // Arrange
        var service = CreateService();
        var postId = Guid.NewGuid();

        _postRepository
            .Setup(x => x.GetByIdAsync(new PostId(postId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        // Act
        var act = () => service.UnlikeAsync(Guid.NewGuid(), postId);

        // Assert
        await act.Should().ThrowAsync<PostNotFoundException>();
    }

    [Fact]
    public async Task UnlikeAsync_Should_Do_Nothing_When_Not_Liked()
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

        // Act
        await service.UnlikeAsync(authorId.Value, post.Id.Value);

        // Assert
        _postLikeRepository.Verify(x => x.Remove(It.IsAny<PostLike>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _postLikeRepository.Verify(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UnlikeAsync_Should_Remove_Like_And_Commit_When_Like_Exists()
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
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _postLikeRepository
            .Setup(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.UnlikeAsync(authorId.Value, post.Id.Value);

        // Assert
        _postLikeRepository.Verify(x => x.Remove(existingLike), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _postLikeRepository.Verify(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
