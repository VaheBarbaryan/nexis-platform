using Modules.Posts.Domain.Authors.ValueObjects;
using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Follows.Rules;

public sealed class CannotFollowYourselfRule : IBusinessRule
{
    private readonly AuthorId _followerId;
    private readonly AuthorId _followeeId;

    public CannotFollowYourselfRule(AuthorId followerId, AuthorId followeeId)
    {
        _followerId = followerId;
        _followeeId = followeeId;
    }

    public bool IsBroken() => _followerId == _followeeId;
    public string Message => "Author cannot follow themselves.";
}
