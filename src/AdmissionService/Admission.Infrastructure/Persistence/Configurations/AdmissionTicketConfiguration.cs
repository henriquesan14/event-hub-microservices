using Admission.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Infrastructure.Persistence.Configurations;

public sealed class AdmissionTicketConfiguration
    : IEntityTypeConfiguration<AdmissionTicket>
{
    public void Configure(EntityTypeBuilder<AdmissionTicket> builder)
    {
        builder.ToTable("admission_tickets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.TicketName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => x.ReservationId);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.CheckInIpAddress).HasMaxLength(64);
    }
}
