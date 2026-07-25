using BuildingBlocks.Contracts.Orders;
using MassTransit;
using Ticketing.Application.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.Infrastructure.Messaging.Consumers;

public sealed class OrderExpiredConsumer(ITicketingRepository repository)
    : IConsumer<OrderExpiredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderExpiredIntegrationEvent> context)
    {
        var reservation = await repository.GetReservationAsync(
            context.Message.ReservationId,
            context.CancellationToken);
        if (reservation is null || reservation.Status != ReservationStatus.Pending)
            return;

        reservation.Release(true);
        reservation.TicketType.Release(reservation.Quantity);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
