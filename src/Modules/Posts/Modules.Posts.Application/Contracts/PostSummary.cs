namespace Modules.Posts.Application.Contracts;

public sealed record PostSummary(
    Guid Id,
    PostAuthor Author,
    string Content,
    DateTimeOffset CreatedAt,
    long LikesCount,
    long CommentsCount);
