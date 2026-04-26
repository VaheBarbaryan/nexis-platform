namespace Modules.Posts.Application.Contracts;

public sealed record FollowSummary(Guid AuthorId, string Username, DateTimeOffset FollowedAt);
