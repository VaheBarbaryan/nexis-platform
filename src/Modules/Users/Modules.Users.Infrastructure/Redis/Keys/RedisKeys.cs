namespace Modules.Users.Infrastructure.Redis.Keys;

public static class RedisKeys
{
    private const string Prefix = "nexis";

    public static string EmailVerification(string tokenHash)
        => $"{Prefix}:users:email-verification:{tokenHash}";
}
