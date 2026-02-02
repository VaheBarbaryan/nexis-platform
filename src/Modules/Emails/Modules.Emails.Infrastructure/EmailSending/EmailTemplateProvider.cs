using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Modules.Emails.Infrastructure.EmailSending;

internal sealed class EmailTemplateProvider : IEmailTemplateProvider
{
    private readonly Assembly _assembly;
    private readonly ILogger<EmailTemplateProvider> _logger;

    private const string ResourceRoot =
        "Modules.Emails.Infrastructure.EmailSending.Templates";

    public EmailTemplateProvider(ILogger<EmailTemplateProvider> logger)
    {
        _assembly = typeof(EmailTemplateProvider).Assembly;
        _logger = logger;
    }

    public async Task<string> GetTemplateAsync(
        string templateName,
        string language,
        CancellationToken cancellationToken)
    {
        var resourcePath =
            $"{ResourceRoot}.{templateName}.{language}.html";

        await using var stream =
            _assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            EmailTemplateProviderLog.LogTemplateNotFound(_logger, templateName, language, null);

            throw new InvalidOperationException(
                $"Email template not found. Resource={resourcePath}");
        }

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
