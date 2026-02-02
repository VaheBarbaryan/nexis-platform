using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modules.Emails.Infrastructure.Configuration;
using Modules.Emails.Infrastructure.EmailSending;
using Modules.Emails.Infrastructure.Kafka;
using SharedKernel.Infrastructure;

namespace Modules.Emails.Infrastructure.ServiceInstallers;

internal sealed class EmailServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = configuration.GetSection("AppSettings");

        EmailTemplateRenderer.SetDefaults(new Dictionary<string, string>
        {
            ["AppName"] = appSettings["AppName"] ?? "Nexis",
            ["SupportEmail"] = appSettings["SupportEmail"] ?? "support@nexis.com"
        });

        services
            .AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var smtpOptions = services.BuildServiceProvider()
            .GetRequiredService<IOptions<SmtpOptions>>().Value;

        services
            .AddFluentEmail(smtpOptions.FromAddress, smtpOptions.FromName)
            .AddMailKitSender(new FluentEmail.MailKitSmtp.SmtpClientOptions
            {
                Server = smtpOptions.Host,
                Port = smtpOptions.Port,
                User = smtpOptions.Username ?? string.Empty,
                Password = smtpOptions.Password ?? string.Empty,
                UseSsl = smtpOptions.UseSsl,
                RequiresAuthentication = smtpOptions.Username != null,
            });

        services.AddScoped<IEmailTemplateProvider, EmailTemplateProvider>();
        services.AddScoped<IEmailSender, FluentEmailSender>();
        services.AddHostedService<EmailBackgroundService>();
    }
}
