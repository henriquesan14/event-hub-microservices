using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;
using BuildingBlocks.Contracts.Orders;
using MassTransit;

namespace Order.Application.Commands.ExpireOrders;

public sealed class ExpireOrdersCommandHandler(
    IOrderRepository repository,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<ExpireOrdersCommand, ResultT<int>>
{
    public async Task<ResultT<int>> Handle(ExpireOrdersCommand request, CancellationToken ct)
    {
        var now = DateTime.Now;
        var orders = await repository.GetExpiredPendingAsync(now, ct);
        var expired = 0;
        foreach (var order in orders)
        {
            order.Expire(now);
            await publishEndpoint.Publish(
                new OrderExpiredIntegrationEvent(
                    order.ReservationId, order.Id, order.ReservationId, order.UserId),
                context => context.CorrelationId = order.ReservationId,
                ct);
            expired++;
        }

        if (expired > 0) await repository.SaveChangesAsync(ct);
        return expired;
    }
}
