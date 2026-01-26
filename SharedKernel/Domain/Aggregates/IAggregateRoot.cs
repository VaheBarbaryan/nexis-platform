using MediatR;

namespace SharedKernel.Domain.Aggregates;

public interface IAggregateRoot
{
    IReadOnlyCollection<INotification> DomainEvents { get; }
    void ClearDomainEvents();
}