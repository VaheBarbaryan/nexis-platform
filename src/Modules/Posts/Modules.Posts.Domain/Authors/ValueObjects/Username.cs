using Modules.Posts.Domain.Authors.Exceptions;

namespace Modules.Posts.Domain.Authors.ValueObjects;

public sealed record Username
{
    public string Value { get; }
    private Username(string value) => Value = value;

    public static Username From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new UsernameCannotBeEmptyException();
        return new Username(value.Trim());
    }
}
