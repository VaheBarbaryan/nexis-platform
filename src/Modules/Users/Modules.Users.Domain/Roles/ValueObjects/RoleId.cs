using SharedKernel.Domain.Entities;

namespace Modules.Users.Domain.Roles.ValueObjects;

public sealed record RoleId : EntityId
{
    private RoleId(Guid value) : base(value)
    {
    }

    public static RoleId New() => new(Guid.NewGuid());
    public static RoleId From(Guid value) => new(value);
}
