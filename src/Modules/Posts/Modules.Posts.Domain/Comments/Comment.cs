using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments.Events;
using Modules.Posts.Domain.Comments.Rules;
using Modules.Posts.Domain.Comments.ValueObjects;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Domain.Aggregates;
using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Comments;

public sealed class Comment : AggregateRoot<CommentId>, ISoftDeletable
{
    public PostId PostId { get; private set; } = null!;
    public AuthorId AuthorId { get; private set; } = null!;
    public CommentContent Content { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    private Comment()
    {
    }

    private Comment(PostId postId, AuthorId authorId, CommentContent content)
    {
        Id = CommentId.New();
        PostId = postId;
        AuthorId = authorId;
        Content = content;

        var now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Comment Create(PostId postId, AuthorId authorId, CommentContent content)
    {
        var comment = new Comment(postId, authorId, content);

        comment.RaiseDomainEvent(
            new CommentCreatedDomainEvent(
                comment.Id.Value,
                comment.PostId.Value,
                comment.AuthorId.Value,
                comment.Content.Value,
                comment.CreatedAt
            )
        );

        return comment;
    }

    public void Update(AuthorId requestingAuthorId, CommentContent content)
    {
        CheckRule(new DeletedCommentCannotBeModifiedRule(IsDeleted));
        CheckRule(new CommentMustBelongToAuthorRule(AuthorId, requestingAuthorId));

        if (Content == content) return;

        Content = content;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(
            new CommentUpdatedDomainEvent(Id.Value, PostId.Value, AuthorId.Value, Content.Value, UpdatedAt));
    }

    public void Delete(AuthorId requestingAuthorId)
    {
        CheckRule(new DeletedCommentCannotBeModifiedRule(IsDeleted));
        CheckRule(new CommentMustBelongToAuthorRule(AuthorId, requestingAuthorId));

        var now = DateTimeOffset.UtcNow;
        DeletedAt = now;
        UpdatedAt = now;

        RaiseDomainEvent(new CommentDeletedDomainEvent(Id.Value, PostId.Value, AuthorId.Value, DeletedAt.Value));
    }
}
