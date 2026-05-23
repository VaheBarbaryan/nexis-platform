using FluentAssertions;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments;
using Modules.Posts.Domain.Comments.Events;
using Modules.Posts.Domain.Comments.ValueObjects;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.UnitTests;

public sealed class CommentTests
{
    private static readonly PostId PostId = PostId.New();
    private static readonly AuthorId AuthorId = AuthorId.New();
    private static readonly CommentContent CommentContent = CommentContent.From("Hello world");

    // -- Create -----------------------------------------------

    [Fact]
    public void Create_Should_Return_Comment_When_Valid_Arguments()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);

        comment.PostId.Should().Be(PostId);
        comment.AuthorId.Should().Be(AuthorId);
        comment.Content.Value.Should().Be("Hello world");
        comment.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_Raise_CommentCreatedDomainEvent()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);

        comment.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<CommentCreatedDomainEvent>();
    }

    [Fact]
    public void Create_Should_Set_CreatedAt_And_UpdatedAt_To_Same_Utc_Time()
    {
        var before = DateTimeOffset.UtcNow;
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        var after = DateTimeOffset.UtcNow;

        comment.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        comment.CreatedAt.Should().Be(comment.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Throw_When_Content_Is_Empty(string content)
    {
        var action = () => Comment.Create(PostId, AuthorId, CommentContent.From(content));

        action.Should().Throw<DomainValidationException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void Create_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        var content = new string('a', 10_001);

        var action = () => Comment.Create(PostId, AuthorId, CommentContent.From(content));

        action.Should().Throw<DomainValidationException>();
    }

    // -- Update -----------------------------------------------

    [Fact]
    public void Update_Should_Update_Content_And_UpdatedAt()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.ClearDomainEvents();

        var before = DateTimeOffset.UtcNow;
        var updatedComment = CommentContent.From("Updated content");
        comment.Update(AuthorId, updatedComment);
        var after = DateTimeOffset.UtcNow;

        comment.Content.Should().Be(updatedComment);
        comment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Update_Should_Raise_CommentUpdatedDomainEvent()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.ClearDomainEvents();

        comment.Update(AuthorId, CommentContent.From("Updated content"));

        comment.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<CommentUpdatedDomainEvent>();
    }

    [Fact]
    public void Update_Should_Throw_When_Comment_Is_Deleted()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.Delete(AuthorId);
        comment.ClearDomainEvents();

        var action = () => comment.Update(AuthorId, CommentContent.From("Updated content"));

        action.Should().Throw<BusinessRuleValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_Should_Throw_When_Content_Is_Empty(string content)
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.ClearDomainEvents();

        var action = () => comment.Update(AuthorId, CommentContent.From(content));

        action.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Content_Exceeds_Max_Length()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.ClearDomainEvents();
        var content = new string('a', 10_001);

        var action = () => comment.Update(AuthorId, CommentContent.From(content));

        action.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Update_Should_Throw_When_Author_Does_Not_Own_Comment()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.ClearDomainEvents();
        var differentAuthorId = AuthorId.New();

        var action = () => comment.Update(differentAuthorId, CommentContent.From("Updated content"));

        action.Should().Throw<BusinessRuleValidationException>();
    }

    // -- Delete -----------------------------------------------

    [Fact]
    public void Delete_Should_Set_DeletedAt_And_Mark_As_Deleted()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
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
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.ClearDomainEvents();

        comment.Delete(AuthorId);

        comment.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<CommentDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_Should_Set_DeletedAt_Once_And_Throw_On_Second_Delete()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        comment.ClearDomainEvents();

        comment.Delete(AuthorId);
        var firstDeletedAt = comment.DeletedAt;

        var secondDelete = () => comment.Delete(AuthorId);
        secondDelete.Should().Throw<BusinessRuleValidationException>();

        comment.DeletedAt.Should().Be(firstDeletedAt);
        comment.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Delete_Should_Throw_When_Author_Does_Not_Own_Comment()
    {
        var comment = Comment.Create(PostId, AuthorId, CommentContent);
        var differentAuthorId = AuthorId.New();

        var action = () => comment.Delete(differentAuthorId);

        action.Should().Throw<BusinessRuleValidationException>();
    }
}
