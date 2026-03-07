using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Posts.Rules;

public sealed class ContentCannotBeEmptyRule : IBusinessRule
{
    private readonly string _content;
    public ContentCannotBeEmptyRule(string content) => _content = content;
    public bool IsBroken() => string.IsNullOrWhiteSpace(_content);
    public string Message => "Post content cannot be empty.";
}
