using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Events;

namespace SharedKernel.Domain.Aggregates;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1030:Use events where appropriate",
    Justification = "This is a domain event helper, not a true C# event")]
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : EntityId
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
