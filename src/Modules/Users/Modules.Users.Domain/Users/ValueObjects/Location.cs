namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record Location
{
    public string Value { get; } = null!;

    private Location()
    {
    }

    private Location(string value) => Value = value;

    public static Location From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Location cannot be empty.", nameof(value));
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > 100)
        {
            throw new ArgumentException("Location cannot exceed 100 characters.", nameof(value));
        }

        return new Location(normalizedValue);
    }
}
