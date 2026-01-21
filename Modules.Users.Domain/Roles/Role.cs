using Modules.Users.Domain.Permissions.ValueObjects;
using Modules.Users.Domain.Roles.Rules;
using Modules.Users.Domain.Roles.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Users.Domain.Roles;

public sealed class Role : AggregateRoot<RoleId>
{
    private readonly List<RolePermission> _permissions = new();

    public RoleName Name { get; private set; }
    
    public IReadOnlyCollection<RolePermission> Permissions => _permissions;
    
    private Role() {}
    
    private Role(RoleId id, RoleName name)
    {
        Id = id;
        Name = name;
    }

    public static Role Create(string name)
    {
        var roleId = new RoleId(Guid.NewGuid());
        var roleName = RoleName.Create(name);
        
        return new Role(roleId, roleName);
    }

    public void AddPermission(PermissionId permissionId)
    {
        _permissions.Add(new RolePermission(Id, permissionId));
    }

    public void RemovePermission(PermissionId permissionId)
    {
        
        CheckRule(new RoleMustHavePermissionRule(_permissions, permissionId));
        
        var permission = _permissions.First(p => p.PermissionId == permissionId);

        _permissions.Remove(permission);
    }
}