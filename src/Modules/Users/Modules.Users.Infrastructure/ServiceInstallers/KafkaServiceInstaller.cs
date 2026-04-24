using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;

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

        services.TryAddSingleton<IEventBusPublisher, KafkaEventBusPublisher>();
    }
}
