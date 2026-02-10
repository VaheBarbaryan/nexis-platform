using StackExchange.Redis;

namespace Modules.Users.Infrastructure.Redis;

public interface ITokenSerializer<T>
{
    RedisValue Serialize(T value);
    T Deserialize(RedisValue value);
}
