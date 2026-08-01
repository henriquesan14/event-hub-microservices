using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.TicketTypeId).IsRequired();
        builder.Property(x => x.EventId).IsRequired();
        builder.Property(x => x.EventName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.EventStartsAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.Quantity);
        builder.Property(x => x.Total).HasPrecision(18, 2);
    }
}
