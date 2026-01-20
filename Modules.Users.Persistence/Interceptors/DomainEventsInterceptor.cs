using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application;
using SharedKernel.Domain.Aggregates;

namespace Modules.Users.Persistence.Interceptors;

public sealed class DomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventsInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        
        var dispatcher = _serviceProvider.GetService<IDomainEventDispatcher>();

        if (dispatcher is null)
        {
            return result;
        }

        if (eventData.Context is null)
        {
            return result;
        }

        var aggregates = eventData.Context
            .ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregates
            .SelectMany(x => x.DomainEvents)
            .ToList();

        aggregates.ForEach(a => a.ClearDomainEvents());

        await dispatcher.DispatchAsync(domainEvents);

        return result;
    }
}