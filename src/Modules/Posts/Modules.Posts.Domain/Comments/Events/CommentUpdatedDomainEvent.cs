using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Comments.Events;

public sealed record CommentUpdatedDomainEvent(
    Guid CommentId,
    Guid PostId,
    Guid AuthorId,
    string Content,
    DateTimeOffset UpdatedAt
) : DomainEvent;
