using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts.Events;
using Modules.Posts.Domain.Posts.Rules;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Domain.Aggregates;
using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Posts;

public sealed class Post : AggregateRoot<PostId>, ISoftDeletable
{
    public AuthorId AuthorId { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    private Post()
    {
    }

    private Post(AuthorId authorId, string content)
    {
        CheckRule(new ContentCannotBeEmptyRule(content));
        CheckRule(new ContentMaxLengthRule(content));

        Id = new PostId(Guid.NewGuid());
        AuthorId = authorId;
        Content = content;

        var now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Post Create(AuthorId authorId, string content)
    {
        var post = new Post(authorId, content);

        post.RaiseDomainEvent(
            new PostCreatedDomainEvent(
                post.Id.Value,
                post.AuthorId.Value,
                post.Content,
                post.CreatedAt
            )
        );

        return post;
    }

    public void Update(AuthorId requestingAuthorId, string content)
    {
        CheckRule(new DeletedPostCannotBeModifiedRule(IsDeleted));
        CheckRule(new PostMustBelongToAuthorRule(AuthorId, requestingAuthorId));
        CheckRule(new ContentCannotBeEmptyRule(content));
        CheckRule(new ContentMaxLengthRule(content));

        Content = content;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new PostUpdatedDomainEvent(Id.Value, AuthorId.Value, Content, UpdatedAt));
    }

    public void Delete()
    {
        if (IsDeleted) return;

        var now = DateTimeOffset.UtcNow;
        DeletedAt = now;
        UpdatedAt = now;

        RaiseDomainEvent(new PostDeletedDomainEvent(Id.Value, AuthorId.Value, DeletedAt.Value));
    }
}
