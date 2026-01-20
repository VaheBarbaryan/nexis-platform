using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .ValueGeneratedNever();
        builder.Property(x => x.Username)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(x => x.Value)
                .HasColumnName("email")
                .HasMaxLength(255)
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
            .IsRequired(false);
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        
        
        builder.HasMany(x => x.Roles)
            .WithOne()
            .HasForeignKey(x => x.UserId);
        
        builder.Navigation(r => r.Roles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(x => x.DomainEvents);
    }
}