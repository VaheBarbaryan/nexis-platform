using SharedKernel.Infrastructure.Messaging;

namespace Modules.Users.Infrastructure.Kafka;

public class KafkaOptions : IKafkaOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string ClientId { get; init; } = "nexis-users";
}
