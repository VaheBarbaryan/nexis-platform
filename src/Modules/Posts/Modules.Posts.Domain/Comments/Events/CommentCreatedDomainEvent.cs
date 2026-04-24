using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Comments.Events;

public sealed record CommentCreatedDomainEvent(
    Guid CommentId,
    Guid PostId,
    Guid AuthorId,
    string Content,
    DateTimeOffset CreatedAt
) : DomainEvent;
