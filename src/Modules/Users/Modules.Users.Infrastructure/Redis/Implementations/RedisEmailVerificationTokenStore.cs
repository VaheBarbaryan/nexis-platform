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
        var key = RedisKeys.EmailVerification(tokenHash);

        await _database.StringSetAsync(
            key,
            userId.ToString(),
            expiry: ttl,
            when: When.Always);
    }

    public async Task<Guid?> GetAsync(
        string tokenHash,
        CancellationToken ct)
    {
        var key = RedisKeys.EmailVerification(tokenHash);
        var value = await _database.StringGetDeleteAsync(key);

        return value.HasValue
            ? Guid.Parse(value!)
            : null;
    }

    public async Task RemoveAsync(
        string tokenHash,
        CancellationToken ct)
    {
        var key = RedisKeys.EmailVerification(tokenHash);
        await _database.KeyDeleteAsync(key);
    }
}
