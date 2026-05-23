namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record Website
{
    public string Value { get; } = null!;

    private Website()
    {
    }

    private Website(string value) => Value = value;

    public static Website From(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Website cannot be empty", nameof(value));

        var normalizedValue = value.Trim();

        if (!Uri.TryCreate(normalizedValue, UriKind.Absolute, out var uriResult) ||
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Invalid website URL format. Must be a valid HTTP or HTTPS address.",
                nameof(value));
        }

        return new Website(normalizedValue);
    }
}
