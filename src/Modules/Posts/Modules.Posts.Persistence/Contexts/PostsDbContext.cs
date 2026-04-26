using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Comments;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Likes;
using Modules.Posts.Domain.Posts;
using SharedKernel.Domain.Entities;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Posts.Persistence.Contexts;

public sealed class PostsDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<PostLike> PostLikes { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Follow> Follows { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(PostsSchema.Name);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostsDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var softDeleteEntries = ChangeTracker
            .Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entityEntry in softDeleteEntries)
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = DateTimeOffset.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
