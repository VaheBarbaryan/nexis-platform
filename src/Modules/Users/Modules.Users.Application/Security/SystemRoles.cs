using Modules.Users.Domain.Roles.ValueObjects;

namespace Modules.Users.Application.Security;

public static class SystemRoles
{
    public static readonly RoleName User = RoleName.From("User");
    public static readonly RoleName Moderator = RoleName.From("Moderator");
    public static readonly RoleName Admin = RoleName.From("Admin");
}
