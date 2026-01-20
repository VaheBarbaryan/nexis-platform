using Modules.Users.Domain.Roles.ValueObjects;
using SharedKernel.Domain.Rules;

namespace Modules.Users.Domain.Users.Rules;

public sealed class UserCannotHaveDuplicateRoleRule : IBusinessRule
{
    private readonly IReadOnlyCollection<UserRole> _roles;
    private readonly RoleId _roleId;

    public UserCannotHaveDuplicateRoleRule(
        IReadOnlyCollection<UserRole> roles,
        RoleId roleId)
    {
        _roles = roles;
        _roleId = roleId;
    }

    public bool IsBroken()
        => _roles.Any(r => r.RoleId == _roleId);

    public string Message
        => "User already has this role assigned.";
}