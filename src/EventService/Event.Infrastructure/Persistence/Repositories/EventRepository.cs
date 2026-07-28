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

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Events
            .FirstOrDefaultAsync(x => x.Id == Events.Domain.ValueObjects.EventId.Of(id), ct);
    }

    public void Delete(Event entity)
    {
        context.Events.Remove(entity);
    }

    public async Task<int> CountAsync(string? title,
        EventStatus? status,
        Guid? ownerId,
        bool includePublished,
        bool includeAll,
        CancellationToken ct)
    {
        var query = context.Events.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Title.Contains(title));

        query = ApplyVisibility(query, ownerId, includePublished, includeAll);

        if (status is not null)
            query = query.Where(x => x.Status == status);

        return await query.CountAsync(ct);
    }

    public async Task<IEnumerable<Event>> GetEvents(string? title,
        EventStatus? status,
        Guid? ownerId,
        bool includePublished,
        bool includeAll,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = context.Events.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Title.Contains(title));

        query = ApplyVisibility(query, ownerId, includePublished, includeAll);

        if (status is not null)
            query = query.Where(x => x.Status == status);

        var items = await query
            .OrderBy(x => x.StartsAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return items;
    }

    private static IQueryable<Event> ApplyVisibility(
        IQueryable<Event> query,
        Guid? ownerId,
        bool includePublished,
        bool includeAll)
    {
        if (includeAll)
            return query;

        if (ownerId is Guid id)
        {
            var organizerId = Events.Domain.ValueObjects.UserId.Of(id);
            return includePublished
                ? query.Where(x =>
                    x.Status == EventStatus.Published ||
                    x.OrganizerId == organizerId)
                : query.Where(x => x.OrganizerId == organizerId);
        }

        return includePublished
            ? query.Where(x => x.Status == EventStatus.Published)
            : query.Where(_ => false);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }
}
