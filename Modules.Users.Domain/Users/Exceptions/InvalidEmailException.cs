using SharedKernel.Domain;
using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public class InvalidEmailException() : DomainException("Email is invalid.");