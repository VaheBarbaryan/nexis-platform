using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .ValueGeneratedNever();

        builder.OwnsOne(x => x.Username, username =>
        {
            username.Property(x => x.Value)
                .HasColumnName("username")
                .HasMaxLength(50)
                .IsRequired();

            username.HasIndex(x => x.Value).IsUnique();
        });

        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(x => x.Value)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            email.HasIndex(x => x.Value).IsUnique();
        });

        builder.OwnsOne(x => x.Password, password =>
        {
            password.Property(x => x.Value)
                .HasColumnName("password")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.Property(x => x.Bio)
            .HasColumnName("bio")
            .HasMaxLength(500);
        builder.Property(x => x.Location)
            .HasColumnName("location")
            .HasMaxLength(100);
        builder.Property(x => x.Website)
            .HasColumnName("website")
            .HasMaxLength(200);
        builder.Property(x => x.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date")
            .IsRequired(false);
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}
