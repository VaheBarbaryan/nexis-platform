using System.Diagnostics.CodeAnalysis;
using Modules.Users.Domain.Users.Exceptions;

namespace Modules.Users.Domain.Users.ValueObjects;

[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
public sealed record Email
{
    public string Value { get; } = null!;

    private Email()
    {
    }

    private Email(string value) => Value = value;

    public static Email From(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidEmailException();

        if (!value.Contains('@', StringComparison.Ordinal)) throw new InvalidEmailException();

        return new Email(value.Trim().ToLowerInvariant());
    }
}
