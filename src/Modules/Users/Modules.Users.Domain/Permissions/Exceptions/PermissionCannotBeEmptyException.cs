using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Permissions.Exceptions;

public sealed class PermissionCannotBeEmptyException : DomainException
{
    public PermissionCannotBeEmptyException()
        : base("Permission cannot be empty.")
    {
    }

    public PermissionCannotBeEmptyException(string message)
        : base(message)
    {
    }

    public PermissionCannotBeEmptyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
