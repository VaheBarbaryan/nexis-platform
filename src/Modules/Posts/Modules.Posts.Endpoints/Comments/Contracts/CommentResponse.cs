using Modules.Posts.Application.Contracts;

namespace Modules.Posts.Endpoints.Comments.Contracts;

public sealed record CommentResponse(
    Guid Id,
    Guid PostId,
    PostAuthor Author,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
