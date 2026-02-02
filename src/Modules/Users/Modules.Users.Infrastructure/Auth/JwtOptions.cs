using System.ComponentModel.DataAnnotations;

namespace Modules.Users.Infrastructure.Auth;

public sealed class JwtOptions
{
    [Required]
    public string Issuer { get; init; } = null!;
    [Required]
    public string Audience { get; init; } = null!;
    [Required]
    public string AccessSecret { get; init; } = null!;
    [Required]
    public string RefreshSecret { get; init; } = null!;
    [Range(1, 1440)]
    public int AccessTokenMinutes { get; init; }
    [Range(1, 365)]
    public int RefreshTokenDays { get; init; }
}
