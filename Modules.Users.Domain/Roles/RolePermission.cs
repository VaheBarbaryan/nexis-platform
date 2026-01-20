using Modules.Users.Domain.Permissions.ValueObjects;
using Modules.Users.Domain.Roles.ValueObjects;

namespace Modules.Users.Domain.Roles;

public class RolePermission
{
    public RoleId RoleId { get; private set; }
    public PermissionId PermissionId { get; private set; }
    
    private RolePermission() {}

    internal RolePermission(RoleId roleId, PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}