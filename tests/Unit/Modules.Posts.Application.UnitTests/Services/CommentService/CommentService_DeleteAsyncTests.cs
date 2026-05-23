using FluentAssertions;
using Modules.Posts.Domain;
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

public sealed class CommentService_DeleteAsyncTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<IPostRepository> _postRepository = new();
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly Mock<IPostUnitOfWork> _unitOfWork = new();

    private Application.Services.CommentService CreateService()
        => new(_commentRepository.Object, _postRepository.Object, _authorRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task DeleteAsync_Should_Throw_When_Comment_Not_Found()
    {
        var service = CreateService();

        _commentRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Domain.Comments.ValueObjects.CommentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        var act = () => service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<CommentNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_Should_Soft_Delete_Comment_And_Commit()
    {
        var service = CreateService();
        var authorId = AuthorId.New();
        var postId = PostId.New();
        var comment = Comment.Create(postId, authorId, CommentContent.From("A comment"));

        _commentRepository
            .Setup(x => x.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _commentRepository
            .Setup(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.DeleteAsync(authorId.Value, comment.Id.Value);

        comment.IsDeleted.Should().BeTrue();
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _commentRepository.Verify(x => x.RefreshCountsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Not_Commit_When_Author_Does_Not_Own_Comment()
    {
        var service = CreateService();
        var authorId = AuthorId.New();
        var differentAuthorId = AuthorId.New();
        var postId = PostId.New();
        var comment = Comment.Create(postId, authorId, CommentContent.From("A comment"));

        _commentRepository
            .Setup(x => x.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var act = () => service.DeleteAsync(differentAuthorId.Value, comment.Id.Value);

        await act.Should().ThrowAsync<Exception>();
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
