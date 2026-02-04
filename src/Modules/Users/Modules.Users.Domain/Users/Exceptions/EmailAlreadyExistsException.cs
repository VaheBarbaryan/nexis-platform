using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public sealed class EmailAlreadyExistsException : ConflictException
{
    public EmailAlreadyExistsException() : base("User with the email already exists.")
    {
    }

    public EmailAlreadyExistsException(string message) : base(message)
    {
    }

    public EmailAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
