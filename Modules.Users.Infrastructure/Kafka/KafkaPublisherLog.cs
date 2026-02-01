using Microsoft.Extensions.Logging;

namespace Modules.Users.Infrastructure.Kafka;

internal static class KafkaPublisherLog
{
    private static readonly Action<ILogger, string, string, int, long, Exception?> _publishedSuccessfully =
        LoggerMessage.Define<string, string, int, long>(
            LogLevel.Information,
            new EventId(1101, nameof(PublishedSuccessfully)),
            "Published {EventType} to {Topic} @ {Partition}:{Offset}");

    public static void PublishedSuccessfully(
        ILogger logger,
        string eventType,
        string topic,
        int partition,
        long offset)
    {
        _publishedSuccessfully(logger, eventType, topic, partition, offset, null);
    }
}
