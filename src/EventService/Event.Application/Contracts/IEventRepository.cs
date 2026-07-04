using Events.Domain.Entities;

namespace EventsApplication.Contracts;

public interface IEventRepository
{
    Task<Event> AddAsync(Event entity, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
