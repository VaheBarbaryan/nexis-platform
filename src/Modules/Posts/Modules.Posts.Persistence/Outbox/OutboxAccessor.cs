using Modules.Posts.Persistence.Contexts;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Posts.Persistence.Outbox;

public class OutboxAccessor : IOutbox
{
    private readonly PostsDbContext _postsDbContext;

    public OutboxAccessor(PostsDbContext postsDbContext)
    {
        _postsDbContext = postsDbContext;
    }

    public void Add(OutboxMessage message)
    {
        _postsDbContext.OutboxMessages.Add(message);
    }
}
