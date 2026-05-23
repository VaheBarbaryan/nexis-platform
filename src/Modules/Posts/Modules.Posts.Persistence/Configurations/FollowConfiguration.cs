using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.ValueObjects;

namespace Modules.Posts.Persistence.Configurations;

public sealed class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("follows");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => FollowId.From(value));

        builder.Property(x => x.FollowerId)
            .HasConversion(
                id => id.Value,
                value => AuthorId.From(value));

        builder.Property(x => x.FolloweeId)
            .HasConversion(
                id => id.Value,
                value => AuthorId.From(value));

        builder.Property(x => x.CreatedAt)
            .HasPrecision(0)
            .IsRequired();

        builder.HasIndex(x => new { x.FollowerId, x.FolloweeId })
            .IsUnique();

        builder.HasOne<Author>()
            .WithMany()
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Author>()
            .WithMany()
            .HasForeignKey(x => x.FolloweeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
