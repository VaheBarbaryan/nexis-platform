using Modules.Users.Domain.Permissions.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Users.Domain.Permissions;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "Permission is a domain entity name")]
public sealed class Permission : Entity<PermissionId>
{
    public PermissionName Name { get; private set; } = null!;

    private Permission()
    {
    }

    private Permission(PermissionName name)
    {
        Id = PermissionId.New();
        Name = name;
    }

    public static Permission Create(PermissionName name)
    {
        return new Permission(name);
    }
}
