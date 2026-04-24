namespace SharedKernel.Infrastructure.Messaging;

public sealed class KafkaProducerOptions : IKafkaProducerOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string ClientId { get; init; } = "nexis-platform";
    public int Retries { get; init; } = 5;
    public string Acks { get; init; } = "all";
}
