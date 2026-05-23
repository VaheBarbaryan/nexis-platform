using SharedKernel.Domain.Entities;

namespace Modules.Users.Domain.Permissions.ValueObjects;

public sealed record PermissionId : EntityId
{
    private PermissionId(Guid value) : base(value)
    {
    }

    public static PermissionId New() => new(Guid.NewGuid());
    public static PermissionId From(Guid value) => new(value);
}
