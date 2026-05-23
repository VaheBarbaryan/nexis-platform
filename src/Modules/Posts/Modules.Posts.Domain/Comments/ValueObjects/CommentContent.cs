using Modules.Posts.Domain.Comments.Exceptions;

namespace Modules.Posts.Domain.Comments.ValueObjects;

public sealed record CommentContent
{
    private const int MaxLength = 10_000;

    public string Value { get; }

    private CommentContent(string value) => Value = value;

    public static CommentContent From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new CommentCannotBeEmptyException();
        }

        if (value.Length > MaxLength)
        {
            throw new CommentContentLengthIsInvalidException();
        }

        return new CommentContent(value);
    }
}
