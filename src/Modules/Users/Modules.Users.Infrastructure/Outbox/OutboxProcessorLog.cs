using Microsoft.Extensions.Logging;

namespace Modules.Users.Infrastructure.Outbox;

internal static class OutboxProcessorLog
{
    private static readonly Action<ILogger, Guid, Exception> _publishFailed =
        LoggerMessage.Define<Guid>(
            LogLevel.Error,
            new EventId(1201, nameof(PublishFailed)),
            "Failed to publish outbox message {MessageId}");

    private static readonly Action<ILogger, Guid, Exception> _networkFailure =
        LoggerMessage.Define<Guid>(
            LogLevel.Warning,
            new EventId(1202, nameof(NetworkFailure)),
            "Network error while publishing outbox message {MessageId}");

    private static readonly Action<ILogger, Guid, Exception> _timeout =
        LoggerMessage.Define<Guid>(
            LogLevel.Warning,
            new EventId(1203, nameof(Timeout)),
            "Timeout while publishing outbox message {MessageId}");

    private static readonly Action<ILogger, Exception> _processorCrashed =
        LoggerMessage.Define(
            LogLevel.Critical,
            new EventId(1299, nameof(ProcessorCrashed)),
            "Outbox processor crashed");

    public static void PublishFailed(ILogger logger, Guid messageId, Exception ex) =>
        _publishFailed(logger, messageId, ex);

    public static void NetworkFailure(ILogger logger, Guid messageId, Exception ex) =>
        _networkFailure(logger, messageId, ex);

    public static void Timeout(ILogger logger, Guid messageId, Exception ex) =>
        _timeout(logger, messageId, ex);

    public static void ProcessorCrashed(ILogger logger, Exception ex) =>
        _processorCrashed(logger, ex);
}
