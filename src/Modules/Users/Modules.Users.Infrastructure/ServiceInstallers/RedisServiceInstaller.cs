using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Redis;
using Modules.Users.Infrastructure.Redis.Implementations;
using Modules.Users.Infrastructure.Redis.Keys;
using Modules.Users.Infrastructure.Redis.Options;
using Modules.Users.Infrastructure.Security;
using SharedKernel.Infrastructure;
using StackExchange.Redis;

namespace Modules.Users.Infrastructure.ServiceInstallers;

internal sealed class RedisServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var redisOptions = configuration
            .GetSection("Redis")
            .Get<RedisOptions>()!;

        var multiplexer =
            ConnectionMultiplexer.Connect(
                redisOptions.ConnectionString);

        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddSingleton<ITokenGenerator, TokenGenerator>();

        services.AddSingleton<ITokenSerializer<Guid>, GuidTokenSerializer>();

        services.AddKeyedScoped<ITokenStore<Guid>>(TokenStoreKey.EmailVerification, (sp, _) =>
        {
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();
            var serializer = sp.GetRequiredService<ITokenSerializer<Guid>>();

            return new RedisTokenStore<Guid>(
                redis,
                RedisKeys.EmailVerification,
                serializer);
        });

        services.AddKeyedScoped<ITokenStore<Guid>>(TokenStoreKey.RefreshToken, (sp, _) =>
        {
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();
            var serializer = sp.GetRequiredService<ITokenSerializer<Guid>>();

            return new RedisTokenStore<Guid>(
                redis,
                RedisKeys.RefreshToken,
                serializer);
        });

        services.AddKeyedScoped<ITokenStore<Guid>>(TokenStoreKey.PasswordReset, (sp, _) =>
        {
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();
            var serializer = sp.GetRequiredService<ITokenSerializer<Guid>>();

            return new RedisTokenStore<Guid>(
                redis,
                RedisKeys.PasswordReset,
                serializer);
        });
    }
}
