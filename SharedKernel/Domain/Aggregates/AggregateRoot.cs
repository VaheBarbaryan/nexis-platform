using MediatR;
using SharedKernel.Domain.Entities;

namespace SharedKernel.Domain.Aggregates;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1030:Use events where appropriate",
    Justification = "This is a domain event helper, not a true C# event")]
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
    private readonly List<INotification> _domainEvents = new();

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents;

    protected void RaiseDomainEvent(INotification domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() =>
        _domainEvents.Clear();
}
