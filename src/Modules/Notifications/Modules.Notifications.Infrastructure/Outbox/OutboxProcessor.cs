using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modules.Notifications.Persistence.Contexts;
using Newtonsoft.Json;
using SharedKernel.Application.Events;
using SharedKernel.Infrastructure.DomainEventsDispatching;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Notifications.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMediator _mediator;
    private readonly IDomainNotificationsMapper _domainNotificationsMapper;
    private readonly ILogger<OutboxProcessor> _logger;

    private const int FrequencyInSeconds = 2;
    private const int BatchSize = 50;
    private const int MaxRetries = 5;

    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(8),
        TimeSpan.FromSeconds(16)
    ];

    public OutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,
        IMediator mediator,
        [FromKeyedServices("notifications")] IDomainNotificationsMapper domainNotificationsMapper,
        ILogger<OutboxProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mediator = mediator;
        _domainNotificationsMapper = domainNotificationsMapper;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(FrequencyInSeconds), stoppingToken);
        }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        var messages = await db.Set<OutboxMessage>()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            bool success = false;

            for (int attempt = message.RetryCount; attempt < MaxRetries; attempt++)
            {
                try
                {
                    var type = _domainNotificationsMapper.GetTypeByName(message.Type);
                    if (type is null)
                        throw new InvalidOperationException($"Unknown outbox message type: {message.Type}");

                    var notification = JsonConvert.DeserializeObject(message.Content, type);

                    if (notification is IDomainEventNotification domainEventNotification)
                        await _mediator.Publish(domainEventNotification, ct);
                    else
                        throw new InvalidOperationException(
                            $"Failed to deserialize outbox message {message.Id} to {type.FullName}");

                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = null;
                    message.RetryCount = attempt;
                    success = true;
                    break;
                }
                catch (HttpRequestException ex)
                {
                    message.RetryCount = attempt + 1;
                    message.Error = ex.Message;
                    OutboxProcessorLog.NetworkFailure(_logger, message.Id, ex);
                }
                catch (TimeoutException ex)
                {
                    message.RetryCount = attempt + 1;
                    message.Error = ex.Message;
                    OutboxProcessorLog.Timeout(_logger, message.Id, ex);
                }

                if (attempt < MaxRetries - 1)
                    await Task.Delay(RetryDelays[attempt], ct);
                else
                    message.ProcessedOnUtc = DateTime.UtcNow;
            }

            _ = success;
        }

        await db.SaveChangesAsync(ct);
    }
}
