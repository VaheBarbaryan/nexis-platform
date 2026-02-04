using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public class DuplicateRoleException : ConflictException
{
    public DuplicateRoleException() : base("Duplicate role")
    {
    }

    public DuplicateRoleException(string message) : base(message)
    {
    }

    public DuplicateRoleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
