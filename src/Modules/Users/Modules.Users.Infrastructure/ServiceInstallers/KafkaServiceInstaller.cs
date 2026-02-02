using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Infrastructure.Kafka;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.EventBus;

namespace Modules.Users.Infrastructure.ServiceInstallers;

internal sealed class KafkaServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<KafkaProducerOptions>()
            .Bind(configuration.GetSection("Kafka"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IEventBusPublisher, KafkaEventBusPublisher>();
    }
}
