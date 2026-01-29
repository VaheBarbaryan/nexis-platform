using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.EventBus;

public interface IEventsBus : IDisposable
{
    Task Publish<T>(T integrationEvent)
        where T : IntegrationEvent;

    void Subscribe<T>(IIntegrationEventProcessor<T> handler)
        where T : IntegrationEvent;

    void StartConsuming();
}
