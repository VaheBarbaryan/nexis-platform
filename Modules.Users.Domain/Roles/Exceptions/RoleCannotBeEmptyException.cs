using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Roles.Exceptions;

public class RoleCannotBeEmptyException : DomainException
{
    public RoleCannotBeEmptyException() : base("Role name cannot be empty.")
    {
    }
}