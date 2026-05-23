using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Notifications.Domain.NotificationActors;
using Modules.Notifications.Domain.NotificationActors.ValueObjects;

namespace Modules.Notifications.Persistence.Configurations;

public sealed class NotificationActorConfiguration : IEntityTypeConfiguration<NotificationActor>
{
    public void Configure(EntityTypeBuilder<NotificationActor> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("notification_actors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => NotificationActorId.From(value)
            );

        builder.Property(x => x.Username)
            .HasConversion(
                username => username.Value,
                value => NotificationUsername.From(value))
            .HasMaxLength(50)
            .IsRequired();
    }
}
