using MediatR;

namespace SharedKernel.Domain.Events;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }

    DateTime OccurredOnUtc { get; }
}
