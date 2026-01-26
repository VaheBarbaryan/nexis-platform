using MediatR;
using SharedKernel.Domain.Entities;

namespace SharedKernel.Domain.Aggregates;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
    private readonly List<INotification> _domainEvents = new();

    public IReadOnlyCollection<INotification> DomainEvents =>
        _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(INotification domainEvent)
    {
        if (domainEvent is null)
        {
            throw new ArgumentNullException(nameof(domainEvent));
        }

        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() =>
        _domainEvents.Clear();
}