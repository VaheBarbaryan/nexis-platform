using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Domain.Users;

public sealed class UserRole {
    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }
    
    private UserRole() { }

    internal UserRole(UserId userId, RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}