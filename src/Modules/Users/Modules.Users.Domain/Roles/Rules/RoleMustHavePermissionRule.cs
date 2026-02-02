using Modules.Users.Domain.Permissions.ValueObjects;
using SharedKernel.Domain.Rules;

namespace Modules.Users.Domain.Roles.Rules;

public sealed class RoleMustHavePermissionRule : IBusinessRule
{
    private readonly IReadOnlyCollection<RolePermission> _permissions;
    private readonly PermissionId _permissionId;

    public RoleMustHavePermissionRule(IReadOnlyCollection<RolePermission> permissions, PermissionId permissionId)
    {
        _permissions = permissions;
        _permissionId = permissionId;
    }
    
    public bool IsBroken()
        => _permissions.All(p => p.PermissionId != _permissionId);
    
    public string Message =>
        "Role does not have the specified permission.";
    
}