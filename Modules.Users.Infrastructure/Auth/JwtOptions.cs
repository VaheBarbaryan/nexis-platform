namespace Modules.Users.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string AccessSecret { get; init; } = null!;
    public string RefreshSecret { get; init; } = null!;
    public int AccessTokenMinutes { get; init; }
    public int RefreshTokenDays { get; init; }
}