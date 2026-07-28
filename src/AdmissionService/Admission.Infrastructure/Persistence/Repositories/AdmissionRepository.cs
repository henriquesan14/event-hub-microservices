using Admission.Application.Contracts;
using Admission.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Admission.Infrastructure.Persistence.Repositories;

public sealed class AdmissionRepository(AdmissionDbContext context)
    : IAdmissionRepository
{
    public async Task AddRangeAsync(
        IEnumerable<AdmissionTicket> tickets,
        CancellationToken ct) =>
        await context.Tickets.AddRangeAsync(tickets, ct);

    public Task<bool> ReservationWasIssuedAsync(Guid reservationId, CancellationToken ct) =>
        context.Tickets.AnyAsync(x => x.ReservationId == reservationId, ct);

    public Task<AdmissionTicket?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.Tickets.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<AdmissionTicket?> GetByCodeAsync(string code, CancellationToken ct) =>
        context.Tickets.FirstOrDefaultAsync(x => x.Code == code, ct);

    public async Task<IReadOnlyList<AdmissionTicket>> GetByUserAsync(
        Guid userId,
        CancellationToken ct) =>
        await context.Tickets
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync(ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}
