using Modules.Users.Domain.Permissions.Exceptions;

namespace Modules.Users.Domain.Permissions.ValueObjects;

public sealed record PermissionName
{
    public string Value { get; }

    private PermissionName(string value)
    {
        Value = value;
    }

    public static PermissionName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PermissionCannotBeEmptyException();

        return new PermissionName(value.ToLowerInvariant());
    }

    public override string ToString() => Value;
}
