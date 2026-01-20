using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public class DuplicateRoleException : DomainException
{
    public DuplicateRoleException() : base("Duplicate role")
    {
    }
}