using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Contracts;
using Ticketing.Domain.Entities;
using Ticketing.Domain.Enums;

namespace Ticketing.Infrastructure.Persistence.Repositories;

public sealed class TicketingRepository(TicketingDbContext context) : ITicketingRepository
{
    public async Task AddTicketTypeAsync(TicketType ticketType, CancellationToken ct) =>
        await context.TicketTypes.AddAsync(ticketType, ct);

    public Task<TicketType?> GetTicketTypeAsync(Guid id, CancellationToken ct) =>
        context.TicketTypes.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<TicketType>> GetTicketTypesByEventAsync(
        Guid eventId, CancellationToken ct) =>
        await context.TicketTypes
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.Price)
            .ToListAsync(ct);

    public void DeleteTicketType(TicketType ticketType) => context.TicketTypes.Remove(ticketType);

    public async Task AddReservationAsync(TicketReservation reservation, CancellationToken ct) =>
        await context.Reservations.AddAsync(reservation, ct);

    public Task<TicketReservation?> GetReservationAsync(Guid id, CancellationToken ct) =>
        context.Reservations
            .Include(x => x.TicketType)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<TicketReservation>> GetExpiredReservationsAsync(
        DateTime now, CancellationToken ct) =>
        await context.Reservations
            .Include(x => x.TicketType)
            .Where(x => x.Status == ReservationStatus.Pending && x.ExpiresAt <= now)
            .ToListAsync(ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
