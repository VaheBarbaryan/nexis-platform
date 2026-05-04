using Microsoft.Extensions.DependencyInjection;
using Modules.Notifications.Domain;
using Modules.Notifications.Persistence.Contexts;
using SharedKernel.Infrastructure.DomainEventsDispatching;

namespace Modules.Notifications.Persistence;

public sealed class NotificationsUnitOfWork : INotificationsUnitOfWork
{
    private readonly NotificationsDbContext _dbContext;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public NotificationsUnitOfWork(
        NotificationsDbContext dbContext,
        [FromKeyedServices("notifications")] IDomainEventsDispatcher domainEventsDispatcher)
    {
        _dbContext = dbContext;
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        await _domainEventsDispatcher.DispatchEventsAsync();
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
