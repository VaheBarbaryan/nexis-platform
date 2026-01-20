using SharedKernel.Application;
using SharedKernel.Domain.Events;

namespace Modules.Users.Infrastructure.Events;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            // TEMP: log or in-memory dispatch
            Console.WriteLine($"Domain Event: {domainEvent.GetType().Name}");
        }

        await Task.CompletedTask;
        
    }
}