using SharedKernel.Domain.Rules;

namespace Modules.Posts.Domain.Posts.Rules;

public sealed class ContentMaxLengthRule : IBusinessRule
{
    private readonly string _content;
    public ContentMaxLengthRule(string content) => _content = content;
    public bool IsBroken() => _content.Length > 500;
    public string Message => "Post content cannot exceed 500 characters.";
}
