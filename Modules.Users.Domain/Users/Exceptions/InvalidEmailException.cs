using SharedKernel.Domain;
using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Domain.Users.Exceptions;

public class InvalidEmailException : DomainException
{
    public InvalidEmailException() : base("Email is invalid.") 
    { 
    }
}