using Modules.Posts.Domain.Authors.ValueObjects;
using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Comments.Rules;

public sealed class CommentMustBelongToAuthorRule : IBusinessRule
{
    private readonly AuthorId _authorId;
    private readonly AuthorId _requestingAuthorId;

    public CommentMustBelongToAuthorRule(AuthorId authorId, AuthorId requestingAuthorId)
    {
        _authorId = authorId;
        _requestingAuthorId = requestingAuthorId;
    }

    public bool IsBroken() => _authorId != _requestingAuthorId;
    public string Message => "The comment must belong to the author performing the action.";
}
