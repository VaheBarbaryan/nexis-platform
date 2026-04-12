using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Likes;
using Modules.Posts.Domain.Likes.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Persistence.Configurations;

public sealed class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("post_likes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new PostLikeId(value));

        builder.Property(x => x.PostId)
            .HasConversion(
                id => id.Value,
                value => new PostId(value));

        builder.Property(x => x.AuthorId)
            .HasConversion(
                id => id.Value,
                value => new AuthorId(value));

        builder.Property(x => x.CreatedAt)
            .HasPrecision(0)
            .IsRequired();

        builder.HasIndex(x => new { x.PostId, x.AuthorId })
            .IsUnique();

        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
