using Modules.Users.Domain.Permissions.ValueObjects;
using SharedKernel.Domain.Rules;

namespace Modules.Users.Domain.Roles.Rules;

public sealed class RoleCannotHaveDuplicatePermissionRule : IBusinessRule
{
    private readonly IReadOnlyCollection<RolePermission> _permissions;
    private readonly PermissionId _permissionId;

    public RoleCannotHaveDuplicatePermissionRule(IReadOnlyCollection<RolePermission> permissions,
        PermissionId permissionId)
    {
        _permissions = permissions;
        _permissionId = permissionId;
    }

    public bool IsBroken()
        => _permissions.Any(p => p.PermissionId == _permissionId);

    public string Message =>
        "Role cannot have duplicate permissions.";
}
