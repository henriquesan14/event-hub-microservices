using Events.Domain.Entities;
using Events.Domain.Enums;

namespace EventsApplication.Contracts;

public interface IEventRepository
{
    Task<Event> AddAsync(Event entity, CancellationToken ct);
    Task<IEnumerable<Event>> GetEvents(string? title,
        EventStatus? status,
        int page,
        int pageSize,
        CancellationToken ct);
    Task<int> CountAsync(string? title,
        EventStatus? status, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
