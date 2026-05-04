using SharedKernel.Infrastructure.Messaging;

namespace Modules.Notifications.Infrastructure.Kafka;

public sealed class KafkaConsumerOptions : IKafkaConsumerOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string ClientId { get; init; } = "nexis-notifications";
    public string GroupId { get; } = "notifications-group";
    public bool EnableAutoCommit { get; }
    public string AutoOffsetReset { get; } = "earliest";
}
