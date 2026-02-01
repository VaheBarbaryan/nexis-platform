using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedKernel.Domain.Events;
using SharedKernel.Infrastructure.Exceptions;
using SharedKernel.Infrastructure.Serialization;

namespace Modules.Users.Infrastructure.Kafka;

public sealed class KafkaEventBusPublisher : IEventBusPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBusPublisher> _logger;

    public KafkaEventBusPublisher(
        IOptions<KafkaOptions> options,
        ILogger<KafkaEventBusPublisher> logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        var kafkaOptions = options.Value ?? throw new ArgumentNullException(nameof(options));

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.BootstrapServers,
            ClientId = kafkaOptions.ClientId,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 5,
            RetryBackoffMs = 500,
            LingerMs = 5,
            CompressionType = CompressionType.Lz4,
        };

        _logger = logger;
        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => Console.WriteLine($"Kafka error: {e}"))
            .SetLogHandler((_, msg) => Console.WriteLine($"Kafka log: {msg.Message}"))
            .Build();
    }

    public async Task PublishAsync<T>(string topic, T integrationEvent, string? partitionKey = null) where T : IntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        var message = new Message<string, string>
        {
            Key = partitionKey ?? Guid.NewGuid().ToString("N"),
            Value = JsonConvert.SerializeObject(integrationEvent, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver(),
                TypeNameHandling = TypeNameHandling.None
            }),
            Headers = new Headers
            {
                { "eventType", System.Text.Encoding.UTF8.GetBytes(integrationEvent.GetType().FullName!) },
                { "occurredOnUtc", System.Text.Encoding.UTF8.GetBytes(integrationEvent.OccurredOnUtc.ToString("O")) }
            }
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, message);

            KafkaPublisherLog.PublishedSuccessfully(
                _logger,
                typeof(T).Name,
                topic,
                deliveryResult.Partition,
                deliveryResult.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            // In outbox pattern this exception should bubble up → job will retry whole message
            throw new KafkaPublishException($"Failed to publish {typeof(T).Name} to {topic}", ex);
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}
