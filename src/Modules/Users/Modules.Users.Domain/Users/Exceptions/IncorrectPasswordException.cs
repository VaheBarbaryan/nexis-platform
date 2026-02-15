using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public sealed class IncorrectPasswordException : DomainValidationException
{
    public IncorrectPasswordException()
        : base("Current password does not match.")
    {
    }

    public IncorrectPasswordException(string message) : base(message)
    {
    }

    public IncorrectPasswordException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
