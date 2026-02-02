using Microsoft.Extensions.Logging;

namespace Modules.Emails.Infrastructure.EmailSending;

public static class EmailTemplateProviderLog
{
    private static readonly EventId TemplateNotFoundEvent = new(1, "TemplateNotFound");

    public static readonly Action<ILogger, string, string, Exception?> LogTemplateNotFound =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            TemplateNotFoundEvent,
            "Email template not found. Template={Template}, Language={Language}");
}
