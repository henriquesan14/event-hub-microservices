using Ticketing.Domain.Entities;

namespace Ticketing.Application.Contracts;

public interface ITicketingRepository
{
    Task AddTicketTypeAsync(TicketType ticketType, CancellationToken ct);
    Task<TicketType?> GetTicketTypeAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TicketType>> GetTicketTypesByEventAsync(Guid eventId, CancellationToken ct);
    void DeleteTicketType(TicketType ticketType);

    Task AddReservationAsync(TicketReservation reservation, CancellationToken ct);
    Task<TicketReservation?> GetReservationAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TicketReservation>> GetExpiredReservationsAsync(DateTime now, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}
