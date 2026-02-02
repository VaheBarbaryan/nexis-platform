using Modules.Users.Domain.Permissions.ValueObjects;
using Modules.Users.Domain.Roles.ValueObjects;

namespace Modules.Users.Domain.Roles;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "RolePermission is a domain aggregate root name")]
public class RolePermission
{
    public RoleId RoleId { get; private set; } = null!;
    public PermissionId PermissionId { get; private set; } = null!;

    private RolePermission() {}

    internal RolePermission(RoleId roleId, PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}
