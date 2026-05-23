using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Persistence.Configurations;

public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => PostId.From(value)
            );

        builder.Property(p => p.Content)
            .HasConversion(
                content => content.Value,
                value => PostContent.From(value))
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.AuthorId)
            .HasConversion(
                id => id.Value,
                value => AuthorId.From(value)
            );

        builder.Property(p => p.CreatedAt)
            .HasPrecision(0)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasPrecision(0)
            .IsRequired();

        builder.Property(p => p.DeletedAt)
            .HasPrecision(0);

        builder.HasOne<Author>()
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => !p.DeletedAt.HasValue);

        builder.Ignore(p => p.IsDeleted);
        builder.Ignore(p => p.DomainEvents);
    }
}
