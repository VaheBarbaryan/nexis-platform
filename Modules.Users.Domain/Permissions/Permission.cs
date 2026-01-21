using Modules.Users.Domain.Permissions.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Users.Domain.Permissions;

public sealed class Permission : AggregateRoot<PermissionId>
{
    public PermissionName Name { get; private set; }
    
    private Permission() {}

    private Permission(PermissionId id, PermissionName name)
    {
        Id = id;
        Name = name;
    }

    public static Permission Create(string name)
    {
        var permission = new Permission(
            new PermissionId(Guid.NewGuid()),
            PermissionName.Create(name)
            );
        
        return permission;
    }
}