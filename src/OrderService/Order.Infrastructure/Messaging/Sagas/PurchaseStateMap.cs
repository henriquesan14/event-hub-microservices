using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Messaging.Sagas;

public sealed class PurchaseStateMap : IEntityTypeConfiguration<PurchaseState>
{
    public void Configure(EntityTypeBuilder<PurchaseState> entity)
    {
        entity.ToTable("PurchaseStates");
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.Currency).HasMaxLength(3);
        entity.Property(x => x.EventName).HasMaxLength(200);
        entity.Property(x => x.FailureReason).HasMaxLength(500);
        entity.Property(x => x.Total).HasPrecision(18, 2);
        entity.HasIndex(x => x.ReservationId).IsUnique();
        entity.HasIndex(x => x.OrderId).IsUnique();
        entity.HasIndex(x => x.PaymentId).IsUnique();
        entity.HasIndex(x => new { x.CurrentState, x.ExpiresAt });
    }
}
