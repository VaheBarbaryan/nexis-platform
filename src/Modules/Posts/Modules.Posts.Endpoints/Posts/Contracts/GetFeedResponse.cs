using Modules.Posts.Application.Contracts;

namespace Modules.Posts.Endpoints.Posts.Contracts;

public sealed record GetFeedResponse(
    Guid Id,
    PostAuthor Author,
    string Content,
    DateTimeOffset CreatedAt,
    long LikesCount,
    long CommentsCount,
    bool IsLiked);
