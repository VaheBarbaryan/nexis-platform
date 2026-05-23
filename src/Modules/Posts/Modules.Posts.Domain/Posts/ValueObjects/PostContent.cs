using Modules.Posts.Domain.Posts.Exceptions;

namespace Modules.Posts.Domain.Posts.ValueObjects;

public sealed record PostContent
{
    private const int MaxLength = 500;

    public string Value { get; }

    private PostContent(string value) => Value = value;

    public static PostContent From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new PostContentCannotBeEmptyException();
        }

        if (value.Length > MaxLength)
        {
            throw new PostContentLengthIsInvalidException();
        }

        return new PostContent(value);
    }
}
