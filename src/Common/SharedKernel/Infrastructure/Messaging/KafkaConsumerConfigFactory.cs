using Confluent.Kafka;

namespace SharedKernel.Infrastructure.Messaging;

public static class KafkaConsumerConfigFactory
{
    public static ConsumerConfig Create(IKafkaConsumerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

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
    }
}
