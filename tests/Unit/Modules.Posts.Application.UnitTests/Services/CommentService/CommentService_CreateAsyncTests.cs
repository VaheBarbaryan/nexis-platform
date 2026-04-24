using FluentAssertions;
using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments.Repositories;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Exceptions;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using Moq;

namespace Modules.Posts.Application.UnitTests.Services.CommentService;

public sealed class CommentService_CreateAsyncTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<IPostRepository> _postRepository = new();
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly Mock<IPostUnitOfWork> _unitOfWork = new();

    private Application.Services.CommentService CreateService()
        => new(_commentRepository.Object, _postRepository.Object, _authorRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Post_Not_Found()
    {
        var service = CreateService();
        var postId = Guid.NewGuid();

        _postRepository
            .Setup(x => x.GetByIdAsync(new PostId(postId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        var act = () => service.CreateAsync(Guid.NewGuid(), postId, "A comment");

        await act.Should().ThrowAsync<PostNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Comment_And_Commit()
    {
        var service = CreateService();
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Post content");
        var author = Author.Create(authorId.Value, "testuser");

        _postRepository
            .Setup(x => x.GetByIdAsync(post.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _authorRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<AuthorId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        await service.CreateAsync(authorId.Value, post.Id.Value, "A comment");

        _commentRepository.Verify(x => x.Add(It.IsAny<Domain.Comments.Comment>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_CommentSummary_With_Author_Info()
    {
        var service = CreateService();
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Post content");
        var author = Author.Create(authorId.Value, "testuser");

        _postRepository
            .Setup(x => x.GetByIdAsync(post.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _authorRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<AuthorId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await service.CreateAsync(authorId.Value, post.Id.Value, "A comment");

        result.Should().BeOfType<CommentSummary>();
        result.PostId.Should().Be(post.Id.Value);
        result.Author.Id.Should().Be(authorId.Value);
        result.Author.Username.Should().Be("testuser");
        result.Content.Should().Be("A comment");
    }
}
