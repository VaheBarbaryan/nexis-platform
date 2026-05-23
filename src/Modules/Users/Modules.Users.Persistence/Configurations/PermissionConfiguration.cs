using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Permissions;
using Modules.Users.Domain.Permissions.ValueObjects;

namespace Modules.Users.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("permissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => PermissionId.From(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasConversion(name => name.Value, value => PermissionName.From(value))
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
