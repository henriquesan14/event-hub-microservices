using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;

namespace Order.Application.Commands.ExpireOrders;

public sealed class ExpireOrdersCommandHandler(
    IOrderRepository repository,
    ITicketingGateway ticketingGateway)
    : ICommandHandler<ExpireOrdersCommand, ResultT<int>>
{
    public async Task<ResultT<int>> Handle(ExpireOrdersCommand request, CancellationToken ct)
    {
        var now = DateTime.Now;
        var orders = await repository.GetExpiredPendingAsync(now, ct);
        var expired = 0;
        foreach (var order in orders)
        {
            if (!await ticketingGateway.ReleaseReservationAsync(order.ReservationId, ct))
                continue;

            order.Expire(now);
            expired++;
        }

        if (expired > 0) await repository.SaveChangesAsync(ct);
        return expired;
    }
}
