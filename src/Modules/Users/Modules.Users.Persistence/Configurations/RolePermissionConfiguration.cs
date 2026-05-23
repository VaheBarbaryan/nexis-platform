using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Permissions;
using Modules.Users.Domain.Permissions.ValueObjects;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Roles.ValueObjects;

namespace Modules.Users.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("role_permissions");

        builder.HasKey(p => new { p.RoleId, p.PermissionId });

        builder.Property(x => x.RoleId)
            .HasConversion(id => id.Value, value => RoleId.From(value));

        builder.Property(x => x.PermissionId)
            .HasConversion(id => id.Value, value => PermissionId.From(value));

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(x => x.PermissionId);
    }
}
