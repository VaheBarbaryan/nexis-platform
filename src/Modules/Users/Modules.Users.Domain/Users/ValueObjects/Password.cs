namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record Password
{
    public string Value { get; } = null!;

    private Password()
    {
    }

    private Password(string value) => Value = value;

    public static Password From(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentException("Password cannot be empty", nameof(hash));

        if (hash.Length < 8) throw new ArgumentException("Password is too short.", nameof(hash));

        return new Password(hash);
    }
}
