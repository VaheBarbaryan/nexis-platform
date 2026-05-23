using Modules.Users.Domain.Permissions.Exceptions;

namespace Modules.Users.Domain.Permissions.ValueObjects;

public sealed record PermissionName
{
    public string Value { get; }

    private PermissionName(string value)
    {
        Value = value;
    }

    public static PermissionName From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new PermissionCannotBeEmptyException();
        }

        var normalized = value.Trim().ToUpperInvariant();

        return new PermissionName(normalized);
    }

    public override string ToString() => Value;
}
