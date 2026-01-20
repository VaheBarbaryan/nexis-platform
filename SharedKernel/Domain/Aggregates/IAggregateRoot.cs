using SharedKernel.Domain.Events;

namespace SharedKernel.Domain.Aggregates;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}