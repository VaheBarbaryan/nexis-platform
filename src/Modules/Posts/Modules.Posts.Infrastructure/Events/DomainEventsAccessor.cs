using Modules.Posts.Persistence.Contexts;
using SharedKernel.Domain.Aggregates;
using SharedKernel.Domain.Events;
using SharedKernel.Infrastructure.DomainEventsDispatching;

namespace Modules.Posts.Infrastructure.Events;

public class DomainEventsAccessor : IDomainEventsAccessor
{
    private readonly PostsDbContext _dbContext;

    public DomainEventsAccessor(PostsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<IDomainEvent> GetAllDomainEvents()
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .ToList();

        return domainEntities
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
