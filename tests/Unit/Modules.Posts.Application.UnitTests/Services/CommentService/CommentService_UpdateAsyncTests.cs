using FluentAssertions;
using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments;
using Modules.Posts.Domain.Comments.Exceptions;
using Modules.Posts.Domain.Comments.Repositories;
using Modules.Posts.Domain.Comments.ValueObjects;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using Moq;

namespace Modules.Posts.Application.UnitTests.Services.CommentService;

public sealed class CommentService_UpdateAsyncTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<IPostRepository> _postRepository = new();
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly Mock<IPostUnitOfWork> _unitOfWork = new();

    private Application.Services.CommentService CreateService()
        => new(_commentRepository.Object, _postRepository.Object, _authorRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task UpdateAsync_Should_Throw_When_Comment_Not_Found()
    {
        var service = CreateService();
        var commentId = Guid.NewGuid();

        _commentRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Domain.Comments.ValueObjects.CommentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        var act = () => service.UpdateAsync(Guid.NewGuid(), commentId, "Updated");

        await act.Should().ThrowAsync<CommentNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Comment_And_Commit()
    {
        var service = CreateService();
        var authorId = AuthorId.New();
        var postId = PostId.New();
        var comment = Comment.Create(postId, authorId, CommentContent.From("Original content"));
        var author = Author.Create(authorId, Username.From("testuser"));

        _commentRepository
            .Setup(x => x.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _authorRepository
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        await service.UpdateAsync(authorId.Value, comment.Id.Value, "Updated content");

        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_CommentSummary_With_Updated_Content()
    {
        var service = CreateService();
        var authorId = AuthorId.New();
        var postId = PostId.New();
        var comment = Comment.Create(postId, authorId, CommentContent.From("Original content"));
        var author = Author.Create(authorId, Username.From("testuser"));

        _commentRepository
            .Setup(x => x.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _authorRepository
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await service.UpdateAsync(authorId.Value, comment.Id.Value, "Updated content");

        result.Should().BeOfType<CommentSummary>();
        result.Content.Should().Be("Updated content");
        result.Author.Username.Should().Be("testuser");
    }
}
