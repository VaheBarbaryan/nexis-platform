using Confluent.Kafka;
using SharedKernel.Infrastructure.Messaging;

namespace Modules.Users.Infrastructure.Kafka;

public sealed class KafkaProducerOptions : IKafkaProducerOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string ClientId { get; init; } = "nexis-users";
    public int Retries { get; } = 5;
    public string Acks { get; } = "all";
}
