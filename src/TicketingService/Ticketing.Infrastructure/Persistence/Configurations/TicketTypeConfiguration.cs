using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Configurations;

public sealed class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.ToTable("ticket_types");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EventId).IsRequired();
        builder.HasIndex(x => x.EventId);
        builder.Property(x => x.EventName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.EventStartsAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.TotalQuantity).IsRequired();
        builder.Property(x => x.AvailableQuantity).IsRequired();
        builder.Property(x => x.SalesStart).HasColumnType("timestamp without time zone");
        builder.Property(x => x.SalesEnd).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Version).IsConcurrencyToken();
    }
}
