using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Posts.Infrastructure.Kafka;
using SharedKernel.Infrastructure;

namespace Modules.Posts.Infrastructure.ServiceInstallers;

internal sealed class KafkaServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<KafkaConsumerOptions>()
            .Bind(configuration.GetSection("Kafka"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHostedService<AuthorBackgroundService>();
    }
}
