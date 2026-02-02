namespace Modules.Users.Infrastructure.Redis.Options;

public sealed class RedisOptions
{
    public string ConnectionString { get; init; } = default!;
}
