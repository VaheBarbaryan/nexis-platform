using StackExchange.Redis;

namespace Modules.Users.Infrastructure.Redis;

public sealed class GuidTokenSerializer : ITokenSerializer<Guid>
{
    public RedisValue Serialize(Guid value) => value.ToString("N");

    public Guid Deserialize(RedisValue value)
    {
        if (!value.HasValue || !Guid.TryParse(value!, out var guid))
        {
            throw new InvalidOperationException("Invalid or missing GUID value in Redis");
        }

        return guid;
    }
}
