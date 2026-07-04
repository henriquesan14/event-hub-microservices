using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Events.Infrastructure.Persistence;

public sealed class EventDbContext : DbContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options)
    : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp");
                }
            }
        }

        foreach (var ownedType in builder.Model.GetEntityTypes().Where(t => t.IsOwned()))
        {
            foreach (var property in ownedType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp");
                }
            }
        }

        base.OnModelCreating(builder);
    }
}
