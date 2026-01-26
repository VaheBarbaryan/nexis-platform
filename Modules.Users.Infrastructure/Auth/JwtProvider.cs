using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Auth;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _jwtOptions = options.Value;
    }
    
    public string GenerateAccessToken(User user)
    {
        var claims = BuildClaims(user);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.AccessSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes)
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.RefreshSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
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
}