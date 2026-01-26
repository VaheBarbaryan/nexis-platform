using System.Text.RegularExpressions;

namespace Modules.Users.Domain.Users.ValueObjects;

public sealed class Username
{
    public string Value { get; }

    private Username() { }

    private Username(string value)
    {
        Value = value;
    }

    public static Username Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty.");
        if (value.Length < 3 || value.Length > 30)
            throw new ArgumentException("Username must be 3-20 characters.");
        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9._]+$"))
            throw new ArgumentException("Username contains invalid characters.");

        return new Username(value);
    }
    
    public override string ToString() => Value;
}