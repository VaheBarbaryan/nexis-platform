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

    // -- Delete -----------------------------------------------

    [Fact]
    public void Delete_Should_Set_DeletedAt_And_Mark_As_Deleted()
    {
        // Arrange
        var post = Post.Create(new AuthorId(Guid.NewGuid()), "Hello world");
        post.ClearDomainEvents();

        // Act
        var before = DateTimeOffset.UtcNow;
        post.Delete();
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
        var post = Post.Create(new AuthorId(Guid.NewGuid()), "Hello world");
        post.ClearDomainEvents();

        // Act
        post.Delete();

        // Assert
        post.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<PostDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_Should_Be_Idempotent_When_Called_Twice()
    {
        // Arrange
        var post = Post.Create(new AuthorId(Guid.NewGuid()), "Hello world");
        post.ClearDomainEvents();

        // Act
        post.Delete();
        var firstDeletedAt = post.DeletedAt;
        post.Delete();

        // Assert
        post.DeletedAt.Should().Be(firstDeletedAt);
        post.DomainEvents.Should().HaveCount(1); // event raised only once
    }
}
