using Modules.Notifications.Persistence.Contexts;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Notifications.Persistence.Outbox;

public sealed class OutboxAccessor : IOutbox
{
    private readonly NotificationsDbContext _dbContext;

    public OutboxAccessor(NotificationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(OutboxMessage message)
    {
        _dbContext.OutboxMessages.Add(message);
    }
}
