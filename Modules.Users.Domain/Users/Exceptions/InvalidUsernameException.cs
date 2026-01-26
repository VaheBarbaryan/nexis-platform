using SharedKernel.Domain;
using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public class InvalidUsernameException() : DomainException("Username cannot be empty or whitespace.");