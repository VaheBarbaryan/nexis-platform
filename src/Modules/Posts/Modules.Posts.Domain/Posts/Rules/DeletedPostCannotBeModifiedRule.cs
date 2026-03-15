using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Posts.Rules;

public sealed class DeletedPostCannotBeModifiedRule : IBusinessRule
{
    private readonly bool _isDeleted;
    public DeletedPostCannotBeModifiedRule(bool isDeleted) => _isDeleted = isDeleted;
    public bool IsBroken() => _isDeleted;
    public string Message => "Cannot update a deleted post.";
}
