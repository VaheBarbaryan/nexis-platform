namespace Modules.Posts.Endpoints.Posts.Contracts;

public sealed record PostResponse(
    Guid Id,
    Guid AuthorId,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    long LikesCount);
