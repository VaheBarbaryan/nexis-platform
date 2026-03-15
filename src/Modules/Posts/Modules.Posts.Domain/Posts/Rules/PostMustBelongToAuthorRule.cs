using Modules.Posts.Domain.Authors.ValueObjects;
using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Posts.Rules;

public sealed class PostMustBelongToAuthorRule : IBusinessRule
{
    private readonly AuthorId _authorId;
    private readonly AuthorId _requestingAuthorId;

    public PostMustBelongToAuthorRule(AuthorId authorId, AuthorId requestingAuthorId)
    {
        _authorId = authorId;
        _requestingAuthorId = requestingAuthorId;
    }

    public bool IsBroken() => _authorId != _requestingAuthorId;
    public string Message => "The post must belong to the author performing the action.";
}
