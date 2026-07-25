using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Domain.Entities.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.OrderId).IsRequired();
        builder.HasIndex(x => x.OrderId).IsUnique();
        builder.Property(x => x.ReservationId).IsRequired();
        builder.HasIndex(x => x.ReservationId);
        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ProviderReference).HasMaxLength(200);
        builder.HasIndex(x => x.ProviderReference).IsUnique();
        builder.Property(x => x.ProviderCustomerReference).HasMaxLength(200);
        builder.Property(x => x.BillingType).HasMaxLength(30);
        builder.Property(x => x.InvoiceUrl).HasMaxLength(1000);
        builder.Property(x => x.FailureReason).HasMaxLength(500);
    }
}
