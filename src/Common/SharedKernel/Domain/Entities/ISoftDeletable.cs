namespace SharedKernel.Domain.Entities;

public interface ISoftDeletable
{
    public DateTimeOffset? DeletedAt { get; }
}
