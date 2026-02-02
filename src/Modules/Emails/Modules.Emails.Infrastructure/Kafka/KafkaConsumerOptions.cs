using SharedKernel.Infrastructure.Messaging;

namespace Modules.Emails.Infrastructure.Kafka;

public sealed class KafkaConsumerOptions : IKafkaConsumerOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string ClientId { get; init; } = "nexis-emails";
    public string GroupId { get; } = "emails-group";
    public bool EnableAutoCommit { get; }
    public string AutoOffsetReset { get; } = "earliest";
}
