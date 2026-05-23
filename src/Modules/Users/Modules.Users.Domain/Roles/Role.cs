using Modules.Users.Domain.Permissions.ValueObjects;
using Modules.Users.Domain.Roles.Rules;
using Modules.Users.Domain.Roles.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Users.Domain.Roles;

public sealed class Role : AggregateRoot<RoleId>
{
    private readonly List<RolePermission> _permissions = new();

    public RoleName Name { get; private set; } = null!;

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role()
    {
    }

    private Role(RoleName name)
    {
        Id = RoleId.New();
        Name = name;
    }

    public static Role Create(RoleName name)
    {
        return new Role(name);
    }

    public void AddPermission(PermissionId permissionId)
    {
        CheckRule(new RoleCannotHaveDuplicatePermissionRule(_permissions, permissionId));

        _permissions.Add(new RolePermission(Id, permissionId));
    }

    public void RemovePermission(PermissionId permissionId)
    {
        CheckRule(new RoleMustHavePermissionRule(_permissions, permissionId));

        var permission = _permissions.First(p => p.PermissionId == permissionId);

        _permissions.Remove(permission);
    }
}
