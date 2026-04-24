using FluentAssertions;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments;
using Modules.Posts.Domain.Comments.Events;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.UnitTests;

public sealed class CommentTests
{
    private static readonly PostId PostId = new(Guid.NewGuid());
    private static readonly AuthorId AuthorId = new(Guid.NewGuid());

    // -- Create -----------------------------------------------

    [Fact]
    public void Create_Should_Return_Comment_When_Valid_Arguments()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");

        comment.PostId.Should().Be(PostId);
        comment.AuthorId.Should().Be(AuthorId);
        comment.Content.Should().Be("Hello world");
        comment.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_Raise_CommentCreatedDomainEvent()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");

        comment.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<CommentCreatedDomainEvent>();
    }

    [Fact]
    public void Create_Should_Set_CreatedAt_And_UpdatedAt_To_Same_Utc_Time()
    {
        var before = DateTimeOffset.UtcNow;
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        var after = DateTimeOffset.UtcNow;

        comment.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        comment.CreatedAt.Should().Be(comment.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Throw_When_Content_Is_Empty(string content)
    {
        var action = () => Comment.Create(PostId, AuthorId, content);

        action.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void Create_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        var content = new string('a', 10_001);

        var action = () => Comment.Create(PostId, AuthorId, content);

        action.Should().Throw<BusinessRuleValidationException>();
    }

    // -- Update -----------------------------------------------

    [Fact]
    public void Update_Should_Update_Content_And_UpdatedAt()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();

        var before = DateTimeOffset.UtcNow;
        comment.Update(AuthorId, "Updated content");
        var after = DateTimeOffset.UtcNow;

        comment.Content.Should().Be("Updated content");
        comment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Update_Should_Raise_CommentUpdatedDomainEvent()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();

        comment.Update(AuthorId, "Updated content");

        comment.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<CommentUpdatedDomainEvent>();
    }

    [Fact]
    public void Update_Should_Throw_When_Comment_Is_Deleted()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.Delete(AuthorId);
        comment.ClearDomainEvents();

        var action = () => comment.Update(AuthorId, "Updated content");

        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_Should_Throw_When_Content_Is_Empty(string content)
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();

        var action = () => comment.Update(AuthorId, content);

        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();
        var content = new string('a', 10_001);

        var action = () => comment.Update(AuthorId, content);

        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Author_Does_Not_Own_Comment()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();
        var differentAuthorId = new AuthorId(Guid.NewGuid());

        var action = () => comment.Update(differentAuthorId, "Updated content");

        action.Should().Throw<BusinessRuleValidationException>();
    }

    // -- Delete -----------------------------------------------

    [Fact]
    public void Delete_Should_Set_DeletedAt_And_Mark_As_Deleted()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();

        var before = DateTimeOffset.UtcNow;
        comment.Delete(AuthorId);
        var after = DateTimeOffset.UtcNow;

        comment.IsDeleted.Should().BeTrue();
        comment.DeletedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        comment.UpdatedAt.Should().Be(comment.DeletedAt);
    }

    [Fact]
    public void Delete_Should_Raise_CommentDeletedDomainEvent()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();

        comment.Delete(AuthorId);

        comment.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<CommentDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_Should_Be_Idempotent_When_Called_Twice()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        comment.ClearDomainEvents();

        comment.Delete(AuthorId);
        var firstDeletedAt = comment.DeletedAt;
        comment.Delete(AuthorId);

        comment.DeletedAt.Should().Be(firstDeletedAt);
        comment.DomainEvents.Should().HaveCount(1); // event raised only once
    }

    [Fact]
    public void Delete_Should_Throw_When_Author_Does_Not_Own_Comment()
    {
        var comment = Comment.Create(PostId, AuthorId, "Hello world");
        var differentAuthorId = new AuthorId(Guid.NewGuid());

        var action = () => comment.Delete(differentAuthorId);

        action.Should().Throw<BusinessRuleValidationException>();
    }
}
