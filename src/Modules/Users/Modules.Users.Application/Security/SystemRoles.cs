using Modules.Users.Domain.Roles.ValueObjects;

namespace Modules.Users.Application.Security;

public static class SystemRoles
{
    public static readonly RoleName User = RoleName.Create("User");
    public static readonly RoleName Moderator = RoleName.Create("Moderator");
    public static readonly RoleName Admin = RoleName.Create("Admin");
}