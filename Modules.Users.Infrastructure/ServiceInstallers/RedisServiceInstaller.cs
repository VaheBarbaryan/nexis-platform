using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Redis.Implementations;
using Modules.Users.Infrastructure.Redis.Options;
using Modules.Users.Infrastructure.Security;
using SharedKernel.Infrastructure;
using StackExchange.Redis;

namespace Modules.Users.Infrastructure.ServiceInstallers;

public class RedisServiceInstaller : IServiceInstaller
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

        services.AddScoped<IEmailVerificationTokenStore, RedisEmailVerificationTokenStore>();
    }
}
