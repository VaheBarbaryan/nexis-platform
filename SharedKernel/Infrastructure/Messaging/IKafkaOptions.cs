namespace SharedKernel.Infrastructure.Messaging;

public interface IKafkaOptions
{
    public string BootstrapServers { get; init; }
    public string ClientId { get; init; }
}
