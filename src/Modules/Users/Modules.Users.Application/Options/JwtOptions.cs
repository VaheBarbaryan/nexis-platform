using System.ComponentModel.DataAnnotations;

namespace Modules.Users.Application.Options;

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
    public int AccessTokenExpirationMinutes { get; init; }
    [Range(1, 365)]
    public int RefreshTokenExpirationDays { get; init; }
}
