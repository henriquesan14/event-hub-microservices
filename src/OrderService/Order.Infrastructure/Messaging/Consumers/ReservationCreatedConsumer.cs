using BuildingBlocks.Contracts.Orders;
using BuildingBlocks.Contracts.Ticketing;
using MassTransit;
using Order.Application.Contracts;

namespace Order.Infrastructure.Messaging.Consumers;

public sealed class ReservationCreatedConsumer(IOrderRepository repository)
    : IConsumer<ReservationCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ReservationCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        if (await repository.ReservationHasOrderAsync(message.ReservationId, context.CancellationToken))
            return;

        if (message.ExpiresAt <= DateTime.Now)
            return;

        var order = Domain.Entities.Order.Create(
            message.UserId,
            message.ReservationId,
            message.TicketTypeId,
            message.EventId,
            message.TicketName,
            message.UnitPrice,
            message.Currency,
            message.Quantity,
            message.ExpiresAt);

        order.CreatedBy = message.UserId;
        order.CreatedByName = message.UserName;

        await repository.AddAsync(order, context.CancellationToken);
        await context.Publish(
            new OrderCreatedIntegrationEvent(
                message.CorrelationId,
                order.Id,
                order.ReservationId,
                order.UserId,
                order.Total,
                order.Currency,
                order.ExpiresAt),
            publish => publish.CorrelationId = message.CorrelationId);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
