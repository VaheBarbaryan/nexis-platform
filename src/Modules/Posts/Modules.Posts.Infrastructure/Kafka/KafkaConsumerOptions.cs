using SharedKernel.Infrastructure.Messaging;

namespace Modules.Posts.Infrastructure.Kafka;

public sealed class KafkaConsumerOptions : IKafkaConsumerOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string ClientId { get; init; } = "nexis-posts";
    public string GroupId { get; } = "posts-group";
    public bool EnableAutoCommit { get; }
    public string AutoOffsetReset { get; } = "earliest";
}
