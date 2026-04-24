using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Comments.Rules;

public sealed class CommentContentMaxLengthRule : IBusinessRule
{
    private readonly string _content;
    public CommentContentMaxLengthRule(string content) => _content = content;
    public bool IsBroken() => _content.Length > 10_000;
    public string Message => "Comment content cannot exceed 10000 characters.";
}
