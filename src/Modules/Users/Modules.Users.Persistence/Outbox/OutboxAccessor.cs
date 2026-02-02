using Modules.Users.Persistence.Contexts;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Users.Persistence.Outbox;

public class OutboxAccessor : IOutbox
{
    private readonly UsersDbContext _usersDbContext;

    public OutboxAccessor(UsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public void Add(OutboxMessage message)
    {
        _usersDbContext.OutboxMessages.Add(message);
    }
}
