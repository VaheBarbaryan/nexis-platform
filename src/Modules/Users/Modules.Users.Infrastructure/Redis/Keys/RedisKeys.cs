using StackExchange.Redis;

namespace Modules.Users.Infrastructure.Redis.Keys;

public static class RedisKeys
{
    private const string Prefix = "nexis";

    public static RedisKey EmailVerification(string tokenHash)
        => $"{Prefix}:users:email-verification:{tokenHash}";

    public static RedisKey RefreshToken(string tokenHash)
        => $"{Prefix}:users:refresh-token:{tokenHash}";

    public static RedisKey PasswordReset(string tokenHash)
        => $"{Prefix}:users:password-reset:{tokenHash}";
}
