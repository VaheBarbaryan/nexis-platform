using Microsoft.EntityFrameworkCore;
using Modules.Notifications.Domain.NotificationActors;
using Modules.Notifications.Domain.Notifications;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Notifications.Persistence.Contexts;

public sealed class NotificationsDbContext : DbContext
{
    public DbSet<NotificationActor> NotificationActors { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(NotificationsSchema.Name);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
    }
}
