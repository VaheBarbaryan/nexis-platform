using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Notifications.Application.Contracts;
using Modules.Users.IntegrationEvents;
using Newtonsoft.Json;
using SharedKernel.Infrastructure.Messaging;
using SharedKernel.Infrastructure.Messaging.Logging;

namespace Modules.Notifications.Infrastructure.Kafka;

public sealed class NotificationActorBackgroundService : BackgroundService
{
    private readonly ILogger<NotificationActorBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<KafkaConsumerOptions> _kafkaConsumerOptions;

    public NotificationActorBackgroundService(
        ILogger<NotificationActorBackgroundService> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaConsumerOptions> kafkaConsumerOptions)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _kafkaConsumerOptions = kafkaConsumerOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = KafkaConsumerConfigFactory.Create(_kafkaConsumerOptions.Value);
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(KafkaTopics.UserEventsV1);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Ignore, string>? cr;

                try
                {
                    cr = await Task.Run(() => consumer.Consume(stoppingToken), stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    KafkaConsumeLog.ConsumeError(_logger, KafkaTopics.UserEventsV1, ex);
                    continue;
                }

                if (cr?.Message?.Value == null) continue;

                try
                {
                    var message = JsonConvert.DeserializeObject<UserEventMessage>(cr.Message.Value);
                    if (message == null) continue;

                    using var scope = _scopeFactory.CreateScope();
                    var actorService = scope.ServiceProvider.GetRequiredService<INotificationActorService>();

                    if (message.Action == "created")
                        await actorService.CreateAsync(message.UserId, message.Username, stoppingToken);

                    consumer.Commit(cr);
                    KafkaConsumeLog.Consumed(_logger, KafkaTopics.UserEventsV1, cr.Message.Key?.ToString());
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
#pragma warning disable CA1031 // intentional: consumer must survive bad messages without dying
                catch (Exception ex)
                {
                    KafkaConsumeLog.ProcessingError(_logger, KafkaTopics.UserEventsV1, ex);
                }
#pragma warning restore CA1031
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
