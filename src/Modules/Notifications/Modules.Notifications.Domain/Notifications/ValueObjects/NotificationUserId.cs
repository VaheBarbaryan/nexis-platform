using SharedKernel.Domain.Entities;

namespace Modules.Notifications.Domain.Notifications.ValueObjects;

public sealed record NotificationUserId : EntityId
{
    private NotificationUserId(Guid value) : base(value) { }
    public static NotificationUserId New() => new(Guid.NewGuid());
    public static NotificationUserId From(Guid value) => new(value);
}
