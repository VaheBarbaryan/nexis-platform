using Modules.Notifications.Persistence.Contexts;
using SharedKernel.Domain.Aggregates;
using SharedKernel.Domain.Events;
using SharedKernel.Infrastructure.DomainEventsDispatching;

namespace Modules.Notifications.Infrastructure.Events;

public sealed class DomainEventsAccessor : IDomainEventsAccessor
{
    private readonly NotificationsDbContext _dbContext;

    public DomainEventsAccessor(NotificationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<IDomainEvent> GetAllDomainEvents()
    {
        return _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();
    }

    public void ClearAllDomainEvents()
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());
    }
}
