using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_roles");
        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.Property(x => x.UserId)
            .HasConversion(id => id.Value, value => UserId.From(value));

        builder.Property(x => x.RoleId)
            .HasConversion(id => id.Value, value => RoleId.From(value));

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(x => x.RoleId);

        builder.HasOne<User>()
            .WithMany(u => u.Roles)
            .HasForeignKey(x => x.UserId);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.RoleId);
    }
}
