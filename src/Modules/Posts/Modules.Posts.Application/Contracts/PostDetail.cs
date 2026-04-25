namespace Modules.Posts.Application.Contracts;

public sealed record PostDetail(
    Guid Id,
    PostAuthor Author,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    long LikesCount,
    long CommentsCount);
