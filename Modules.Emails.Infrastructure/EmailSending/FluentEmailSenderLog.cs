using Microsoft.Extensions.Logging;

namespace Modules.Emails.Infrastructure.EmailSending;

internal static class FluentEmailSenderLog
{
    private static readonly Action<ILogger, string, string, Exception?> _logEmailSendFailed = LoggerMessage.Define<string, string>(
        LogLevel.Error,
        new EventId(1, "EmailSendFailed"),
        "Email send failed. Recipient={Recipient}. Error={Error}");

    private static readonly Action<ILogger, string, Exception?> _logEmailSent = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(2, "EmailSent"),
        "Email sent successfully. Recipient={Recipient}");

    public static void EmailSendFailed(this ILogger logger, string recipient, string error) =>
        _logEmailSendFailed(logger, recipient, error, null);

    public static void EmailSent(this ILogger logger, string recipient) =>
        _logEmailSent(logger, recipient, null);
}
