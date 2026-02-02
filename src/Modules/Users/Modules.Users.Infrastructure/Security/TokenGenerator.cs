using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Modules.Users.Application.Contracts;

namespace Modules.Users.Infrastructure.Security;

public sealed class TokenGenerator : ITokenGenerator
{
    private readonly byte[] _pepper;

    public TokenGenerator(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        _pepper = Encoding.UTF8.GetBytes(
            config["Security:TokenPepper"] ?? string.Empty);
    }

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return WebEncoders.Base64UrlEncode(bytes);
    }

    public string Hash(string token)
    {
        using var hmac = new HMACSHA256(_pepper);

        var hash = hmac.ComputeHash(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(hash);
    }

    public bool Verify(string token, string hash)
        => Hash(token) == hash;
}
