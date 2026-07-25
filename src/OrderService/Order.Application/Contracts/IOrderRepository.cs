using Order.Domain.Entities;

namespace Order.Application.Contracts;

public interface IOrderRepository
{
    Task AddAsync(Domain.Entities.Order order, CancellationToken ct);
    Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ReservationHasOrderAsync(Guid reservationId, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Order>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Order>> GetExpiredPendingAsync(DateTime now, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
