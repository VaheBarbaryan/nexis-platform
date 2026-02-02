using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.EventBus;

public interface IIntegrationEventConsumer<in TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    Task Consume(TIntegrationEvent integrationEvent);
}
