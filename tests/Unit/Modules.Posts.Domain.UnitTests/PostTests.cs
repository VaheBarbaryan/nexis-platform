using FluentAssertions;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Events;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.UnitTests;

public sealed class PostTests
{
    private static readonly AuthorId AuthorId = AuthorId.New();

    // -- Create -----------------------------------------------

    [Fact]
    public void Create_Should_Return_Post_When_Valid_Arguments()
    {
        // Arrange
        var postContent = PostContent.From("Hello world");

        // Act
        var post = Post.Create(AuthorId, postContent);

        // Assert
        post.AuthorId.Should().Be(AuthorId);
        post.Content.Should().Be(postContent);
        post.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_Raise_PostCreatedDomainEvent()
    {
        // Arrange
        var postContent = PostContent.From("Hello world");

        // Act
        var post = Post.Create(AuthorId, postContent);

        // Assert
        post.DomainEvents
            .Should()
            .ContainSingle()
            .Which.Should().BeOfType<PostCreatedDomainEvent>();
    }

    [Fact]
    public void Create_Should_Set_CreatedAt_And_UpdatedAt_To_Same_Utc_Time()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        var after = DateTimeOffset.UtcNow;

        // Assert
        post.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        post.CreatedAt.Should().Be(post.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Throw_When_Content_Is_Empty(string content)
    {
        var action = () => Post.Create(AuthorId, PostContent.From(content));

        action.Should().Throw<DomainValidationException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void Create_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        // Arrange
        var content = new string('a', 501);

        // Act
        var action = () => Post.Create(AuthorId, PostContent.From(content));

        // Assert
        action.Should().Throw<DomainValidationException>();
    }

    // -- Update -----------------------------------------------

    [Fact]
    public void Update_Should_Update_Content_And_UpdatedAt()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello World"));
        post.ClearDomainEvents();

        // Act
        var before = DateTimeOffset.UtcNow;
        var updatedContent = PostContent.From("Updated content");
        post.Update(AuthorId, updatedContent);
        var after = DateTimeOffset.UtcNow;

        // Assert
        post.Content.Should().Be(updatedContent);
        post.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Update_Should_Raise_PostUpdatedDomainEvent()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.ClearDomainEvents();

        // Act
        post.Update(AuthorId, PostContent.From("Updated content"));

        // Assert
        post.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<PostUpdatedDomainEvent>();
    }

    [Fact]
    public void Update_Should_Throw_When_Post_Is_Deleted()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.Delete(AuthorId);
        post.ClearDomainEvents();

        // Act
        var action = () => post.Update(AuthorId, PostContent.From("Updated content"));

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_Should_Throw_When_Content_Is_Empty(string content)
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.ClearDomainEvents();

        // Act
        var action = () => post.Update(AuthorId, PostContent.From(content));

        // Assert
        action.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.ClearDomainEvents();
        var content = new string('a', 501);

        // Act
        var action = () => post.Update(AuthorId, PostContent.From(content));

        // Assert
        action.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Author_Does_Not_Own_Post()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.ClearDomainEvents();
        var differentAuthorId = AuthorId.New();

        // Act
        var action = () => post.Update(differentAuthorId, PostContent.From("Updated content"));

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }

    // -- Delete -----------------------------------------------

    [Fact]
    public void Delete_Should_Set_DeletedAt_And_Mark_As_Deleted()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.ClearDomainEvents();

        // Act
        var before = DateTimeOffset.UtcNow;
        post.Delete(AuthorId);
        var after = DateTimeOffset.UtcNow;

        // Assert
        post.IsDeleted.Should().BeTrue();
        post.DeletedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        post.UpdatedAt.Should().Be(post.DeletedAt);
    }

    [Fact]
    public void Delete_Should_Raise_PostDeletedDomainEvent()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.ClearDomainEvents();

        // Act
        post.Delete(AuthorId);

        // Assert
        post.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<PostDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_Should_Be_Idempotent_When_Called_Twice()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        post.ClearDomainEvents();

        // Act
        post.Delete(AuthorId);
        var firstDeletedAt = post.DeletedAt;
        post.Delete(AuthorId);

        // Assert
        post.DeletedAt.Should().Be(firstDeletedAt);
        post.DomainEvents.Should().HaveCount(1); // event raised only once
    }

    [Fact]
    public void Delete_Should_Throw_When_Author_Does_Not_Own_Post()
    {
        // Arrange
        var post = Post.Create(AuthorId, PostContent.From("Hello world"));
        var differentAuthorId = AuthorId.New();

        // Act
        var action = () => post.Delete(differentAuthorId);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }
}
