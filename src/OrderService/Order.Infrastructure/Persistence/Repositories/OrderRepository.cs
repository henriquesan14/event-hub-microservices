using Microsoft.EntityFrameworkCore;
using Order.Application.Contracts;
using Order.Domain.Enums;

namespace Order.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(OrderDbContext context) : IOrderRepository
{
    public async Task AddAsync(Domain.Entities.Order order, CancellationToken ct) =>
        await context.Orders.AddAsync(order, ct);

    public Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ReservationHasOrderAsync(Guid reservationId, CancellationToken ct) =>
        context.Orders.AnyAsync(x => x.ReservationId == reservationId, ct);

    public async Task<IReadOnlyList<Domain.Entities.Order>> GetByUserAsync(
        Guid userId, CancellationToken ct) =>
        await context.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Domain.Entities.Order>> GetExpiredPendingAsync(
        DateTime now, CancellationToken ct) =>
        await context.Orders
            .Where(x => x.Status == OrderStatus.PendingPayment && x.ExpiresAt <= now)
            .ToListAsync(ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
