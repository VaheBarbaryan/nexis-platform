
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Redis.Keys;
using StackExchange.Redis;

namespace Modules.Users.Infrastructure.Redis.Implementations;

public sealed class RedisEmailVerificationTokenStore : IEmailVerificationTokenStore
{
    private readonly IDatabase _database;

    public RedisEmailVerificationTokenStore(
        IConnectionMultiplexer redis)
    {
        ArgumentNullException.ThrowIfNull(redis);

        _database = redis.GetDatabase();
    }
    public async Task StoreAsync(
        Guid userId,
        string tokenHash,
        TimeSpan ttl,
        CancellationToken ct)
    {
        var key = RedisKeys.EmailVerification(userId);

        await _database.StringSetAsync(
            key,
            tokenHash,
            expiry: ttl,
            when: When.Always);
    }

    public async Task<string?> GetAsync(
        Guid userId,
        CancellationToken ct)
    {
        var key = RedisKeys.EmailVerification(userId);

        var value = await _database.StringGetAsync(key);

        return value.HasValue
            ? value.ToString()
            : null;
    }

    public async Task RemoveAsync(
        Guid userId,
        CancellationToken ct)
    {
        var key = RedisKeys.EmailVerification(userId);

        await _database.KeyDeleteAsync(key);
    }
}
