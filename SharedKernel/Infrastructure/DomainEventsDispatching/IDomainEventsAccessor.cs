using MediatR;

namespace SharedKernel.Infrastructure.DomainEventsDispatching;

public interface IDomainEventsAccessor
{
    IReadOnlyCollection<INotification> GetAllDomainEvents();

    void ClearAllDomainEvents();
}