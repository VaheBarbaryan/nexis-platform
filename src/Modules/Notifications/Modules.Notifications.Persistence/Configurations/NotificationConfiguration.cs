using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Notifications.Domain.Notifications;
using Modules.Notifications.Domain.Notifications.Enums;
using Modules.Notifications.Domain.Notifications.ValueObjects;

namespace Modules.Notifications.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => NotificationId.From(value));

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RecipientId)
            .HasConversion(
                recipientId => recipientId.Value,
                value => NotificationUserId.From(value))
            .IsRequired();
        builder.Property(x => x.ActorId)
            .HasConversion(
                actorId => actorId.Value,
                value => NotificationUserId.From(value))
            .IsRequired();
        builder.Property(x => x.EntityId)
            .HasConversion(
                entityId => entityId.Value,
                value => NotificationEntityId.From(value))
            .IsRequired();
        builder.Property(x => x.IsRead).IsRequired();
        builder.Property(x => x.CreatedAt).HasPrecision(0).IsRequired();

        builder.HasIndex(x => x.RecipientId);
        builder.HasIndex(x => new { x.RecipientId, x.IsRead });
    }
}
