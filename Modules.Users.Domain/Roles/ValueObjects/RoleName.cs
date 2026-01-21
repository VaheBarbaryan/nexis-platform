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

        return new RoleName(value.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
