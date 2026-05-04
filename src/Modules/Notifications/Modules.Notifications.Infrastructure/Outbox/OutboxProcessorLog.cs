using Microsoft.Extensions.Logging;

namespace Modules.Notifications.Infrastructure.Outbox;

internal static class OutboxProcessorLog
{
    private static readonly Action<ILogger, Guid, Exception> _networkFailure =
        LoggerMessage.Define<Guid>(
            LogLevel.Warning,
            new EventId(3202, nameof(NetworkFailure)),
            "Network error while publishing outbox message {MessageId}");

    private static readonly Action<ILogger, Guid, Exception> _timeout =
        LoggerMessage.Define<Guid>(
            LogLevel.Warning,
            new EventId(3203, nameof(Timeout)),
            "Timeout while publishing outbox message {MessageId}");

    public static void NetworkFailure(ILogger logger, Guid messageId, Exception ex) =>
        _networkFailure(logger, messageId, ex);

    public static void Timeout(ILogger logger, Guid messageId, Exception ex) =>
        _timeout(logger, messageId, ex);
}
