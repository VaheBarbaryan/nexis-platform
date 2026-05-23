using SharedKernel.Domain.Entities;

namespace Modules.Users.Domain.Users.ValueObjects;

public sealed record UserId : EntityId
{
    private UserId(Guid value) : base(value)
    {
    }

    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);
}
