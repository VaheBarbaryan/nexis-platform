using Autofac;
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Redis.Implementations;
using Modules.Users.Infrastructure.Redis.Keys;
using StackExchange.Redis;

namespace Modules.Users.Infrastructure.Redis;

public sealed class RedisModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(ctx =>
            {
                var redis = ctx.Resolve<IConnectionMultiplexer>();
                var serializer = ctx.Resolve<ITokenSerializer<Guid>>();

                return new RedisTokenStore<Guid>(
                    redis,
                    RedisKeys.EmailVerification,
                    serializer);
            })
            .Keyed<ITokenStore<Guid>>(TokenStoreKey.EmailVerification)
            .InstancePerLifetimeScope();

        builder.Register(ctx =>
            {
                var redis = ctx.Resolve<IConnectionMultiplexer>();
                var serializer = ctx.Resolve<ITokenSerializer<Guid>>();

                return new RedisTokenStore<Guid>(
                    redis,
                    RedisKeys.RefreshToken,
                    serializer);
            })
            .Keyed<ITokenStore<Guid>>(TokenStoreKey.RefreshToken)
            .InstancePerLifetimeScope();

        builder.Register(ctx =>
            {
                var redis = ctx.Resolve<IConnectionMultiplexer>();
                var serializer = ctx.Resolve<ITokenSerializer<Guid>>();

                return new RedisTokenStore<Guid>(
                    redis,
                    RedisKeys.PasswordReset,
                    serializer);
            })
            .Keyed<ITokenStore<Guid>>(TokenStoreKey.PasswordReset)
            .InstancePerLifetimeScope();
    }
}
