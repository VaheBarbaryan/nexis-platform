namespace Modules.Emails.Infrastructure.Configuration;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public bool UseSsl { get; set; }

    public string? Username { get; set; }
    public string? Password { get; set; }

    public string FromAddress { get; set; } = null!;
    public string FromName { get; set; } = null!;
}
