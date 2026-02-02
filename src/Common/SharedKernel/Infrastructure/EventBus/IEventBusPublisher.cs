using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.EventBus;

public interface IEventBusPublisher
{
    Task PublishAsync<T>(string topic, T integrationEvent, string? partitionKey = null)
        where T : IntegrationEvent;
}
