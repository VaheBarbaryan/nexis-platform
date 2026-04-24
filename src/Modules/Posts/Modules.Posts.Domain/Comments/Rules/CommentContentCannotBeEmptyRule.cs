using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Comments.Rules;

public sealed class CommentContentCannotBeEmptyRule : IBusinessRule
{
    private readonly string _content;
    public CommentContentCannotBeEmptyRule(string content) => _content = content;
    public bool IsBroken() => string.IsNullOrWhiteSpace(_content);
    public string Message => "Comment content cannot be empty.";
}
