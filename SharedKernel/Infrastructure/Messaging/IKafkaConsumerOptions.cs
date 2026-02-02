namespace SharedKernel.Infrastructure.Messaging;

public interface IKafkaConsumerOptions
{
    string BootstrapServers { get; }
    string ClientId { get; }
    string GroupId { get; }
    bool EnableAutoCommit { get; }
    string AutoOffsetReset { get; }
}
