using Microsoft.Extensions.Logging;

namespace SharedKernel.Infrastructure.Messaging.Logging;

public static class KafkaConsumeLog
{
    private static readonly Action<ILogger, string, string?, Exception?> _consumed =
        LoggerMessage.Define<string, string?>(
            LogLevel.Information,
            new EventId(1201, nameof(Consumed)),
            "Kafka consumed message. Topic={Topic} Key={Key}");

    private static readonly Action<ILogger, string, Exception?> _consumeError =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1202, nameof(ConsumeError)),
            "Kafka consume error. Topic={Topic}");

    public static void Consumed(ILogger logger, string topic, string? key)
    {
        _consumed(logger, topic, key, null);
    }

    public static void ConsumeError(ILogger logger, string topic, Exception exception)
    {
        _consumeError(logger, topic, exception);
    }
}
