using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Comments.Rules;

public sealed class DeletedCommentCannotBeModifiedRule : IBusinessRule
{
    private readonly bool _isDeleted;
    public DeletedCommentCannotBeModifiedRule(bool isDeleted) => _isDeleted = isDeleted;
    public bool IsBroken() => _isDeleted;
    public string Message => "Cannot update a deleted comment.";
}
