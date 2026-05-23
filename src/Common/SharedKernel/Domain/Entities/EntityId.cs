namespace SharedKernel.Domain.Entities;

public abstract record EntityId
{
    public Guid Value { get; }

    protected EntityId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(value));
        }

        Value = value;
    }
}
