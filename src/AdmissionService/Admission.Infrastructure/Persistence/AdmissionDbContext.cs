using Admission.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Admission.Infrastructure.Persistence;

public sealed class AdmissionDbContext(DbContextOptions<AdmissionDbContext> options)
    : DbContext(options)
{
    public DbSet<AdmissionTicket> Tickets => Set<AdmissionTicket>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AdmissionDbContext).Assembly);
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
