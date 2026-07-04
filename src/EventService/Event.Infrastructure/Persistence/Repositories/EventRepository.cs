using Events.Domain.Entities;
using Events.Domain.Enums;
using EventsApplication.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Persistence.Repositories;

public sealed class EventRepository(EventDbContext context) : IEventRepository
{
    public async Task<Event> AddAsync(Event entity, CancellationToken ct)
    {
        await context.Set<Event>().AddAsync(entity, ct);
        return entity;
    }

    public async Task<int> CountAsync(string? title,
        EventStatus? status, CancellationToken ct)
    {
        var query = context.Events.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Title.Contains(title));

        if (status is not null)
            query = query.Where(x => x.Status == status);

        return await query.CountAsync(ct);
    }

    public async Task<IEnumerable<Event>> GetEvents(string? title,
        EventStatus? status,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = context.Events.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Title.Contains(title));

        if (status is not null)
            query = query.Where(x => x.Status == status);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.StartsAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return items;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }
}
