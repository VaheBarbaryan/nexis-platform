using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Permissions.Exceptions;

public class PermissionNotFoundException : DomainException
{
    public PermissionNotFoundException(string name) : base($"Permission '{name}' not found while building system roles")
    {
    }
}