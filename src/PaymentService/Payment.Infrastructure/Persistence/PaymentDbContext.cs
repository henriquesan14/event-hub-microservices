using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Payment.Infrastructure.Persistence;

public sealed class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Payment> Payments => Set<Domain.Entities.Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.Namespace?.StartsWith("MassTransit", StringComparison.Ordinal) == true)
                continue;

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetColumnType("timestamp without time zone");
            }
        }

        base.OnModelCreating(builder);
    }
}
