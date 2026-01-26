using MediatR;
using SharedKernel.Infrastructure.DomainEventsDispatching;

namespace Modules.Users.Infrastructure.Events;

public sealed class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IDomainEventsAccessor _domainEventsAccessor;
    private readonly IMediator _mediator;

    public DomainEventsDispatcher(
        IDomainEventsAccessor domainEventsAccessor, 
        IMediator mediator)
    {
        _domainEventsAccessor = domainEventsAccessor ?? throw new ArgumentNullException(nameof(domainEventsAccessor));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task DispatchEventsAsync()
    {
        var domainEvents = _domainEventsAccessor.GetAllDomainEvents();

        // 4️⃣ Dispatch all events asynchronously
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
        
        _domainEventsAccessor.ClearAllDomainEvents();
    }
}