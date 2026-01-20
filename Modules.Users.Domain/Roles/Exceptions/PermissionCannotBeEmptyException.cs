using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Roles.Exceptions;

public class PermissionCannotBeEmptyException : DomainException
{
    public PermissionCannotBeEmptyException() : base("Permission cannot be empty.")
    {
    }
}