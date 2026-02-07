using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Redis.Keys;
using StackExchange.Redis;

namespace Modules.Users.Infrastructure.Redis.Implementations;

public sealed class RefreshTokenStore : IRefreshTokenStore
{
    private readonly IDatabase _database;

    public RefreshTokenStore(IConnectionMultiplexer redis)
    {
        ArgumentNullException.ThrowIfNull(redis);

        _database = redis.GetDatabase();
    }

    public async Task StoreAsync(
        string userId,
        string refreshTokenHash,
        TimeSpan expiresIn,
        CancellationToken ct = default)
    {
        var key = RedisKeys.RefreshToken(refreshTokenHash);

        await _database.StringSetAsync(
            key,
            userId,
            expiry: expiresIn,
            when: When.Always);
    }

    public async Task<Guid?> GetAsync(string refreshTokenHash, CancellationToken ct = default)
    {
        var key = RedisKeys.RefreshToken(refreshTokenHash);
        var value = await _database.StringGetAsync(key);

        return value.HasValue
            ? Guid.Parse(value!)
            : null;
    }

    public async Task RemoveAsync(string refreshTokenHash, CancellationToken ct = default)
    {
        var key = RedisKeys.RefreshToken(refreshTokenHash);
        await _database.KeyDeleteAsync(key);
    }
}
