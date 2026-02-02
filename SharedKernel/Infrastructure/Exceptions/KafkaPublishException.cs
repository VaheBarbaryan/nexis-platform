using SharedKernel.Domain.Exceptions;

namespace SharedKernel.Infrastructure.Exceptions;

public class KafkaPublishException : DomainException
{
    public KafkaPublishException() : base("Kafka Publish Exception")
    {
    }

    public KafkaPublishException(string message) : base(message)
    {
    }

    public KafkaPublishException(string message, Exception inner) : base(message, inner)
    {
    }
}
