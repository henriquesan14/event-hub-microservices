namespace Events.Infrastructure.Persistence.Configurations;

using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => TicketId.Of(value));

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(x => x.Amount)
                 .HasColumnName("Price");

            money.Property(x => x.Currency)
                 .HasMaxLength(3);
        });

        builder.Property(x => x.Quantity);

        builder.Property(x => x.AvailableQuantity);
    }
}
