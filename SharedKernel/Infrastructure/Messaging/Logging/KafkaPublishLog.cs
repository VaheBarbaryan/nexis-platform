using Microsoft.Extensions.Logging;

namespace SharedKernel.Infrastructure.Messaging.Logging;

public static class KafkaPublishLog
{
    private static readonly Action<ILogger, string, string, int, long, Exception?> _publishedSuccessfully =
        LoggerMessage.Define<string, string, int, long>(
            LogLevel.Information,
            new EventId(1101, nameof(PublishedSuccessfully)),
            "Kafka publish success. Event={EventType} Topic={Topic} Partition={Partition} Offset={Offset}");

    private static readonly Action<ILogger, string, string, Exception?> _publishFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(1102, nameof(PublishFailed)),
            "Kafka publish failed. Event={EventType} Topic={Topic}");

    public static void PublishedSuccessfully(
        ILogger logger,
        string eventType,
        string topic,
        int partition,
        long offset)
    {
        _publishedSuccessfully(logger, eventType, topic, partition, offset, null);
    }

    public static void PublishFailed(
        ILogger logger,
        string eventType,
        string topic,
        Exception exception)
    {
        _publishFailed(logger, eventType, topic, exception);
    }
}
