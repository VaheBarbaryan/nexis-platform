namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record BirthDate
{
    public DateOnly Value { get; }

    private BirthDate()
    {
    }

    private BirthDate(DateOnly value) => Value = value;

    public static BirthDate From(DateOnly value)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (value > today)
        {
            throw new ArgumentException("Birth date cannot be in the future.", nameof(value));
        }

        if (value < today.AddYears(-125))
        {
            throw new ArgumentException("Birth date is outside a valid historical range.", nameof(value));
        }

        return new BirthDate(value);
    }
}
