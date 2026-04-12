namespace Modules.Posts.Application.Contracts;

public sealed record PostDetail(
    Guid Id,
    Guid AuthorId,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    long LikesCount);
