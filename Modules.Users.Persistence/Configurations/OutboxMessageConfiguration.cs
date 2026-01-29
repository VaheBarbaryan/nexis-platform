using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Users.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("outbox_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

        builder.Property(x => x.Error)
            .IsRequired(false)
            .HasMaxLength(2000);

        // Index for fast lookup of unprocessed messages
        builder.HasIndex(x => x.ProcessedOnUtc);
        builder.HasIndex(x => x.OccurredOnUtc);
    }
}
