using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modules.Emails.Infrastructure.Kafka;
using SharedKernel.Infrastructure;

namespace Modules.Emails.Infrastructure.ServiceInstallers;

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

        services.AddSingleton<ConsumerConfig>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaConsumerOptions>>().Value;

            return new ConsumerConfig
            {
                BootstrapServers = options.BootstrapServers,
                ClientId = options.ClientId,
                GroupId = options.GroupId,
                EnableAutoCommit = options.EnableAutoCommit,
                AutoOffsetReset = options.AutoOffsetReset.ToUpperInvariant() switch
                {
                    "EARLIEST" => AutoOffsetReset.Earliest,
                    "LATEST" => AutoOffsetReset.Latest,
                    _ => AutoOffsetReset.Earliest
                }
            };
        });
    }
}
