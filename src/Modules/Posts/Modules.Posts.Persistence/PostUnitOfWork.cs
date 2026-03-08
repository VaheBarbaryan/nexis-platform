using Modules.Posts.Domain;
using Modules.Posts.Persistence.Contexts;

namespace Modules.Posts.Persistence;

public sealed class PostUnitOfWork : IPostUnitOfWork
{
    private readonly PostsDbContext _dbContext;

    public PostUnitOfWork(PostsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
