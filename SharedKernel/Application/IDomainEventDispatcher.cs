using SharedKernel.Domain.Events;

namespace SharedKernel.Application;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents);
}