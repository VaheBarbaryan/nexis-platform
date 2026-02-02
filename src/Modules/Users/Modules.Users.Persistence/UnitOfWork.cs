using Modules.Users.Persistence.Contexts;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.DomainEventsDispatching;

namespace Modules.Users.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly UsersDbContext _dbContext;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public UnitOfWork(UsersDbContext dbContext, IDomainEventsDispatcher domainEventsDispatcher)
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
