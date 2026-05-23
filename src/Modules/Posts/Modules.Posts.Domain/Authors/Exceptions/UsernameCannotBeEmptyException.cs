using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.Authors.Exceptions;

public sealed class UsernameCannotBeEmptyException : DomainValidationException
{
    public UsernameCannotBeEmptyException()
        : base("Author username cannot be empty.")
    {
    }

    public UsernameCannotBeEmptyException(string message)
        : base(message)
    {
    }

    public UsernameCannotBeEmptyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
