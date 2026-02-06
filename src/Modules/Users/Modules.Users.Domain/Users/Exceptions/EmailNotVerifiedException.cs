using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public sealed class EmailNotVerifiedException : DomainValidationException
{
    public EmailNotVerifiedException()
        : base("Email must be verified before login.")
    {
    }

    public EmailNotVerifiedException(string message) : base(message)
    {
    }

    public EmailNotVerifiedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
