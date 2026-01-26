using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public sealed class EmailAlreadyExistsException() : DomainException("User with the email already exists.")
{
}