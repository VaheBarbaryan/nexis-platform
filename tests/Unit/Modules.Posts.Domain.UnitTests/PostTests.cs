using FluentAssertions;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Events;
using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.UnitTests;

public sealed class PostTests
{
    // -- Create -----------------------------------------------

    [Fact]
    public void Create_Should_Return_Post_When_Valid_Arguments()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());

        // Act
        var post = Post.Create(authorId, "Hello world");

        // Assert
        post.AuthorId.Should().Be(authorId);
        post.Content.Should().Be("Hello world");
        post.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_Raise_PostCreatedDomainEvent()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());

        // Act
        var post = Post.Create(authorId, "Hello world");

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
        var post = Post.Create(new AuthorId(Guid.NewGuid()), "Hello world");
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
        var action = () => Post.Create(new AuthorId(Guid.NewGuid()), content);

        action.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void Create_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        // Arrange
        var content = new string('a', 501);

        // Act
        var action = () => Post.Create(new AuthorId(Guid.NewGuid()), content);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }

    // -- Update -----------------------------------------------

    [Fact]
    public void Update_Should_Update_Content_And_UpdatedAt()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.ClearDomainEvents();

        // Act
        var before = DateTimeOffset.UtcNow;
        post.Update(authorId, "Updated content");
        var after = DateTimeOffset.UtcNow;

        // Assert
        post.Content.Should().Be("Updated content");
        post.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Update_Should_Raise_PostUpdatedDomainEvent()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.ClearDomainEvents();

        // Act
        post.Update(authorId, "Updated content");

        // Assert
        post.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<PostUpdatedDomainEvent>();
    }

    [Fact]
    public void Update_Should_Throw_When_Post_Is_Deleted()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.Delete(authorId);
        post.ClearDomainEvents();

        // Act
        var action = () => post.Update(authorId, "Updated content");

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_Should_Throw_When_Content_Is_Empty(string content)
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.ClearDomainEvents();

        // Act
        var action = () => post.Update(authorId, content);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.ClearDomainEvents();
        var content = new string('a', 501);

        // Act
        var action = () => post.Update(authorId, content);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Author_Does_Not_Own_Post()
    {
        // Arrange
        var post = Post.Create(new AuthorId(Guid.NewGuid()), "Hello world");
        post.ClearDomainEvents();
        var differentAuthorId = new AuthorId(Guid.NewGuid());

        // Act
        var action = () => post.Update(differentAuthorId, "Updated content");

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }

    // -- Delete -----------------------------------------------

    [Fact]
    public void Delete_Should_Set_DeletedAt_And_Mark_As_Deleted()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.ClearDomainEvents();

        // Act
        var before = DateTimeOffset.UtcNow;
        post.Delete(authorId);
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
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.ClearDomainEvents();

        // Act
        post.Delete(authorId);

        // Assert
        post.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<PostDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_Should_Be_Idempotent_When_Called_Twice()
    {
        // Arrange
        var authorId = new AuthorId(Guid.NewGuid());
        var post = Post.Create(authorId, "Hello world");
        post.ClearDomainEvents();

        // Act
        post.Delete(authorId);
        var firstDeletedAt = post.DeletedAt;
        post.Delete(authorId);

        // Assert
        post.DeletedAt.Should().Be(firstDeletedAt);
        post.DomainEvents.Should().HaveCount(1); // event raised only once
    }

    [Fact]
    public void Delete_Should_Throw_When_Author_Does_Not_Own_Post()
    {
        // Arrange
        var post = Post.Create(new AuthorId(Guid.NewGuid()), "Hello world");
        var differentAuthorId = new AuthorId(Guid.NewGuid());

        // Act
        var action = () => post.Delete(differentAuthorId);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>();
    }
}
