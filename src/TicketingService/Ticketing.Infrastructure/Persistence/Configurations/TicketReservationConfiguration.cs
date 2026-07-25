using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Configurations;

public sealed class TicketReservationConfiguration : IEntityTypeConfiguration<TicketReservation>
{
    public void Configure(EntityTypeBuilder<TicketReservation> builder)
    {
        builder.ToTable("ticket_reservations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.TicketTypeId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId);
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.HasIndex(x => new { x.Status, x.ExpiresAt });

        builder.HasOne(x => x.TicketType)
            .WithMany()
            .HasForeignKey(x => x.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
