using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.EventBus;

public interface IIntegrationEventProcessor<in TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent integrationEvent);
}
