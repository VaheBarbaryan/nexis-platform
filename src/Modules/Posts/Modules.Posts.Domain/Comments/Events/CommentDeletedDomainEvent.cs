using SharedKernel.Domain.Events;

namespace Modules.Posts.Domain.Comments.Events;

public sealed record CommentDeletedDomainEvent(
    Guid CommentId,
    Guid PostId,
    Guid AuthorId,
    DateTimeOffset DeletedAt
) : DomainEvent;
