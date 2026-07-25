using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Persistence.Configurations;

public sealed class ProcessedWebhookEventConfiguration
    : IEntityTypeConfiguration<ProcessedWebhookEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedWebhookEvent> builder)
    {
        builder.ToTable("processed_webhook_events");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(200).ValueGeneratedNever();
        builder.Property(x => x.EventType).HasMaxLength(100);
        builder.Property(x => x.ProcessedAt);
    }
}
