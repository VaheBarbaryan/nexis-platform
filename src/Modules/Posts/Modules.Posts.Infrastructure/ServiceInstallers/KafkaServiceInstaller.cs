using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modules.Posts.Infrastructure.Kafka;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

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

        services
            .AddOptions<KafkaProducerOptions>()
            .Bind(configuration.GetSection("Kafka"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddSingleton<IEventBusPublisher, KafkaEventBusPublisher>();

        services.AddHostedService<AuthorBackgroundService>();
    }
}
