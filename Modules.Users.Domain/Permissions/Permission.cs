using Modules.Users.Domain.Permissions.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Users.Domain.Permissions;

public sealed class Permission : AggregateRoot<PermissionId>
{
    public string Name { get; private set; }
    
    private Permission() {}

    private Permission(string name)
    {
        Name = name;
    }

    public static Permission Create(string name)
    {
        var permission = new Permission(name);
        
        return permission;
    }
}