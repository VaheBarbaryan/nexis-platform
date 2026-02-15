using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public sealed class PasswordReuseException : DomainValidationException
{
    public PasswordReuseException()
        : base("New password cannot be the same as the old password")
    {
    }

    public PasswordReuseException(string message) : base(message)
    {
    }

    public PasswordReuseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
