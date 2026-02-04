namespace SharedKernel.Domain.Exceptions;

public abstract class DomainValidationException : DomainException
{
    protected DomainValidationException() : base("Validation error.") { }

    protected DomainValidationException(string message) : base(message) { }

    protected DomainValidationException(string message, Exception innerException) : base(message, innerException) { }
}
