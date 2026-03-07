using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.ValueObjects;

namespace Modules.Posts.Persistence.Configurations;

public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("authors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new AuthorId(value)
            );

        builder.Property(x => x.Username)
            .HasMaxLength(50)
            .IsRequired();
    }
}
