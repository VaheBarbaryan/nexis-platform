using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Permissions;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Users;

namespace Modules.Users.Persistence.Contexts;

public sealed class UsersDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}