namespace Modules.Posts.Application.Contracts;

public sealed record PostSummary(
    Guid Id,
    Guid AuthorId,
    string Content,
    DateTimeOffset CreatedAt);
