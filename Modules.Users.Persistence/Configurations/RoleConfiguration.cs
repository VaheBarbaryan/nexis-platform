using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Roles.ValueObjects;

namespace Modules.Users.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new RoleId(value));
        builder.Property(r => r.Name)
            .HasConversion(name => name.Value, value => RoleName.Create(value))
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasMany(r => r.Permissions)
            .WithOne()
            .HasForeignKey(x => x.RoleId);

        builder.Navigation(r => r.Permissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}