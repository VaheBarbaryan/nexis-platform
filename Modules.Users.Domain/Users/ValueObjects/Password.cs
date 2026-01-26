namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record Password
{
    public string Value { get; }
    
    private Password() {}

    private Password(string value)
    {
        Value = value;
    }

    public static Password Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Password cannot be empty");

        if (hash.Length < 8)
            throw new ArgumentException("Password is too short.");
        
        return new Password(hash);
    }
}