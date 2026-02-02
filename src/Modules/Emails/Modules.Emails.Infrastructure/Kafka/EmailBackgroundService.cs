using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modules.Emails.Infrastructure.EmailSending;
using Newtonsoft.Json;
using SharedKernel.Infrastructure.Messaging;
using SharedKernel.Infrastructure.Messaging.Commands;
using SharedKernel.Infrastructure.Messaging.Logging;

namespace Modules.Emails.Infrastructure.Kafka;

public class EmailBackgroundService : BackgroundService
{
    private readonly ILogger<EmailBackgroundService> _logger;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly ConsumerConfig _consumerConfig;

    public EmailBackgroundService(
        ConsumerConfig consumerConfig,
        ILogger<EmailBackgroundService> logger,
        IEmailSender emailSender,
        IEmailTemplateProvider emailTemplateProvider
    )
    {
        _logger = logger;
        _emailSender = emailSender;
        _consumerConfig = consumerConfig;
        _emailTemplateProvider = emailTemplateProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe(KafkaTopics.NotificationsEmailV1);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    if (cr?.Message?.Value == null) continue;

                    var command = JsonConvert.DeserializeObject<SendEmailIntegrationCommand>(cr.Message.Value);
                    if (command == null) continue;

                    var template = await _emailTemplateProvider.GetTemplateAsync(
                        command.Template,
                        command.Language,
                        stoppingToken
                    );

                    var htmlBody = EmailTemplateRenderer.Render(template, command.Variables);

                    await _emailSender.SendAsync(
                        command.To,
                        command.Subject,
                        htmlBody,
                        stoppingToken
                    );

                    consumer.Commit(cr);

                    KafkaConsumeLog.Consumed(_logger, KafkaTopics.NotificationsEmailV1, cr.Message.Key?.ToString());
                }
                catch (ConsumeException ex)
                {
                    KafkaConsumeLog.ConsumeError(_logger, KafkaTopics.NotificationsEmailV1, ex);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
