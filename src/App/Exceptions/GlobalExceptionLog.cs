namespace App.Exceptions;

internal static class GlobalExceptionLog
{
    private static readonly Action<ILogger, Exception> _exceptionError =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1401, nameof(ExceptionError)),
            "Unhandled exception occurred.");

    public static void ExceptionError(ILogger logger, Exception ex) =>
        _exceptionError(logger, ex);
}
