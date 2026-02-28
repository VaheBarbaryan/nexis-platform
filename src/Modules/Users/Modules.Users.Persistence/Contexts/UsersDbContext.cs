using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Permissions;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Users;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Users.Persistence.Contexts;

public sealed class UsersDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(UsersSchema.Name);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}
