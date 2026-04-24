using Microsoft.Extensions.DependencyInjection;
using Modules.Posts.Domain;
using Modules.Posts.Persistence.Contexts;
using SharedKernel.Infrastructure.DomainEventsDispatching;

namespace Modules.Posts.Persistence;

public sealed class PostUnitOfWork : IPostUnitOfWork
{
    private readonly PostsDbContext _dbContext;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public PostUnitOfWork(
        PostsDbContext dbContext,
        [FromKeyedServices("posts")] IDomainEventsDispatcher domainEventsDispatcher)
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
