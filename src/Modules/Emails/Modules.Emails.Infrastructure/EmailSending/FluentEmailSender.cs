using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace Modules.Emails.Infrastructure.EmailSending;

public sealed class FluentEmailSender : IEmailSender
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<FluentEmailSender> _logger;

    public FluentEmailSender(
        IFluentEmail fluentEmail,
        ILogger<FluentEmailSender> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public async Task SendAsync(
        string recipient,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(htmlBody);

        var response = await _fluentEmail
            .To(recipient)
            .Subject(subject)
            .Body(htmlBody, isHtml: true)
            .SendAsync(cancellationToken);

        Console.WriteLine($"Response: {response}");

        if (!response.Successful)
        {
            var error = string.Join(", ", response.ErrorMessages);

            FluentEmailSenderLog.EmailSendFailed(_logger, recipient, error);

            throw new InvalidOperationException($"Email send failed: {error}");
        }

        FluentEmailSenderLog.EmailSent(_logger, recipient);
    }
}
