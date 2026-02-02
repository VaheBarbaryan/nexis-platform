namespace SharedKernel.Infrastructure.Messaging;

public interface IKafkaProducerOptions
{
    string BootstrapServers { get; }
    string ClientId { get; }
    int Retries { get; }
    string Acks { get; }
}
