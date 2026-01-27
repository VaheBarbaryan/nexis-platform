using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Roles.Exceptions;

public class RoleCannotBeEmptyException : DomainException
{
    public RoleCannotBeEmptyException() : base("Role name cannot be empty.")
    {
    }

    public RoleCannotBeEmptyException(string message) : base(message)
    {
    }

    public RoleCannotBeEmptyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
