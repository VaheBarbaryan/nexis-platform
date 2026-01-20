using Modules.Users.Domain.Users.Exceptions;

namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email() { }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException();

        if (!value.Contains('@'))
            throw new InvalidEmailException();

        return new Email(value.Trim().ToLowerInvariant());
    }
}