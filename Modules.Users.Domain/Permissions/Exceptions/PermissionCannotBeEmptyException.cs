using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Permissions.Exceptions;

public class PermissionCannotBeEmptyException : DomainException
{
    public PermissionCannotBeEmptyException() : base("Permission cannot be empty.")
    {
    }
}