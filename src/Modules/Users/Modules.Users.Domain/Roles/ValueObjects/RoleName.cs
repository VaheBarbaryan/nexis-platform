using Modules.Users.Domain.Roles.Exceptions;

namespace Modules.Users.Domain.Roles.ValueObjects;

public sealed record RoleName
{
    public string Value { get; }

    private RoleName(string value)
    {
        Value = value;
    }

    public static RoleName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new RoleCannotBeEmptyException();

        var normalized = value.Trim().ToUpperInvariant();

        return new RoleName(normalized);
    }

    public override string ToString() => Value;
}
