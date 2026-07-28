using Admission.Domain.Entities;

namespace Admission.Application.Contracts;

public interface IAdmissionRepository
{
    Task AddRangeAsync(IEnumerable<AdmissionTicket> tickets, CancellationToken ct);
    Task<bool> ReservationWasIssuedAsync(Guid reservationId, CancellationToken ct);
    Task<AdmissionTicket?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AdmissionTicket?> GetByCodeAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<AdmissionTicket>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
