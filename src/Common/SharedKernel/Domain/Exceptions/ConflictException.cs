namespace SharedKernel.Domain.Exceptions;

public abstract class ConflictException : DomainException
{
    protected ConflictException() : base("Conflict error.") { }

    protected ConflictException(string message) : base(message) { }

    protected ConflictException(string message, Exception innerException) : base(message, innerException) { }
}
