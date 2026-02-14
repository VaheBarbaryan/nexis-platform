using Modules.Users.Application.Contracts;
using StackExchange.Redis;

namespace Modules.Users.Infrastructure.Redis.Implementations;

public sealed class RedisTokenStore<TIdentifier> : ITokenStore<TIdentifier>
{
    private readonly IDatabase _database;
    private readonly Func<string, RedisKey> _keyFactory;
    private readonly ITokenSerializer<TIdentifier> _tokenSerializer;

    public RedisTokenStore(
        IConnectionMultiplexer connectionMultiplexer,
        Func<string, RedisKey> keyFactory,
        ITokenSerializer<TIdentifier> tokenSerializer)
    {
        ArgumentNullException.ThrowIfNull(connectionMultiplexer);

        _database = connectionMultiplexer.GetDatabase();
        _keyFactory = keyFactory;
        _tokenSerializer = tokenSerializer;
    }

    public async Task StoreAsync(
        TIdentifier identifier,
        string tokenHash,
        TimeSpan ttl,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (ttl <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ttl),
                "TTL must be positive");
        }

        var key = _keyFactory(tokenHash);
        var value = _tokenSerializer.Serialize(identifier);

        await _database.StringSetAsync(
            key,
            value,
            expiry: ttl,
            when: When.Always);
    }

    public async Task<TIdentifier?> GetAsync(
        string tokenHash,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        var key = _keyFactory(tokenHash);
        var value = await _database.StringGetAsync(key);

        return value.HasValue
            ? _tokenSerializer.Deserialize(value!)
            : default;
    }

    public async Task<TIdentifier?> GetDeleteAsync(
        string tokenHash,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        var key = _keyFactory(tokenHash);
        var value = await _database.StringGetDeleteAsync(key);

        return value.HasValue
            ? _tokenSerializer.Deserialize(value!)
            : default;
    }

    public async Task RemoveAsync(
        string tokenHash,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        var key = _keyFactory(tokenHash);
        await _database.KeyDeleteAsync(key);
    }
}
