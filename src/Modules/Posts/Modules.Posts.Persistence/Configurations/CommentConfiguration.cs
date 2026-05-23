using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments;
using Modules.Posts.Domain.Comments.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => CommentId.From(value));

        builder.Property(c => c.PostId)
            .HasConversion(
                id => id.Value,
                value => PostId.From(value));

        builder.Property(c => c.AuthorId)
            .HasConversion(
                id => id.Value,
                value => AuthorId.From(value));

        builder.Property(c => c.Content)
            .HasConversion(
                content => content.Value,
                value => CommentContent.From(value)
            )
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasPrecision(0)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasPrecision(0)
            .IsRequired();

        builder.Property(c => c.DeletedAt)
            .HasPrecision(0);

        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Author>()
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.PostId, c.CreatedAt });

        builder.HasQueryFilter(c => !c.DeletedAt.HasValue);

        builder.Ignore(c => c.IsDeleted);
        builder.Ignore(c => c.DomainEvents);
    }
}
