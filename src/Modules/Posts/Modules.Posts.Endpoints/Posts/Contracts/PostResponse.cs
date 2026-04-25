using Modules.Posts.Application.Contracts;

namespace Modules.Posts.Endpoints.Posts.Contracts;

public sealed record PostResponse(
    Guid Id,
    PostAuthor Author,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    long LikesCount,
    long CommentsCount);
