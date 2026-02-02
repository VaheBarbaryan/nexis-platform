namespace Modules.Emails.Infrastructure.EmailSending;

/// <summary>
/// Resolves a raw HTML template string by name and language.
/// Implementations can back this with files, a database, a blob store, etc.
/// </summary>
public interface IEmailTemplateProvider
{
    /// <param name="templateName">The template key, e.g. "order-confirmation".</param>
    /// <param name="language">The language/locale code, e.g. "en", "hy".</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The raw HTML template string with placeholder tokens still in place.</returns>
    /// <exception cref="InvalidOperationException">Template not found for the given name and language.</exception>
    Task<string> GetTemplateAsync(string templateName, string language, CancellationToken cancellationToken);
}
