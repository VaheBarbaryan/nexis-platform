using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Auth;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly SigningCredentials _accessCredentials;
    private readonly SigningCredentials _refreshCredentials;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _jwtOptions = options.Value;
        _accessCredentials = CreateCredentials(_jwtOptions.AccessSecret);
        _refreshCredentials = CreateCredentials(_jwtOptions.RefreshSecret);
    }

    public string GenerateAccessToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = BuildClaims(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = _accessCredentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = _refreshCredentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays)
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    private static List<Claim> BuildClaims(User user)
    {
        return [
            new Claim("username", user.Username.Value),
            new Claim("email", user.Email.Value),
            new Claim("sub", user.Id.Value.ToString())
        ];
    }

    private static SigningCredentials CreateCredentials(string secret)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
    }
}
