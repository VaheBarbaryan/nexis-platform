namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record Bio
{
    private const int MaxLength = 100;

    public string Value { get; } = null!;

    private Bio()
    {
    }

    private Bio(string value) => Value = value;

    public static Bio From(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Bio cannot be empty", nameof(value));

        if (value.Length > MaxLength)
        {
            throw new ArgumentException($"Bio length cannot exceed {MaxLength} characters.", nameof(value));
        }

        return new Bio(value);
    }
}
