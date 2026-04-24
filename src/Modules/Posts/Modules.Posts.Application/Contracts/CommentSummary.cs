namespace Modules.Posts.Application.Contracts;

public sealed record CommentSummary(
    Guid Id,
    Guid PostId,
    CommentAuthor Author,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
