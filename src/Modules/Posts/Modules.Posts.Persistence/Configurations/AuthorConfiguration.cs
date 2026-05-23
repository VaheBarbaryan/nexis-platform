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
                value => AuthorId.From(value)
            );

        builder.Property(x => x.Username)
            .HasConversion(
                username => username.Value,
                value => Username.From(value)
            )
            .HasMaxLength(50)
            .IsRequired();
    }
}
