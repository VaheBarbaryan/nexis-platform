using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Roles.Exceptions;

public sealed class RoleNotFoundException : NotFoundException
{
    public RoleNotFoundException()
        : base("Role not found.") { }

    public RoleNotFoundException(string roleName)
        : base($"Role '{roleName}' not found.") { }

    public RoleNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
