using Microsoft.EntityFrameworkCore;
using Ticketing.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence;

public sealed class TicketingDbContext(DbContextOptions<TicketingDbContext> options) : DbContext(options)
{
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<TicketReservation> Reservations => Set<TicketReservation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(TicketingDbContext).Assembly);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetColumnType("timestamp without time zone");
            }
        }

        base.OnModelCreating(builder);
    }
}
