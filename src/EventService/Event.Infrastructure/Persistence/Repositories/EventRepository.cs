using Events.Domain.Entities;
using EventsApplication.Contracts;

namespace Events.Infrastructure.Persistence.Repositories;

public sealed class EventRepository(EventDbContext context) : IEventRepository
{
    public async Task<Event> AddAsync(Event entity, CancellationToken ct)
    {
        await context.AddAsync(entity, ct);
        return entity;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }
}
