using BuildingBlocks.Contracts.Payments;
using MassTransit;
using Ticketing.Application.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.Infrastructure.Messaging.Consumers;

public sealed class PaymentFailedConsumer(ITicketingRepository repository)
    : IConsumer<PaymentFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
    {
        var reservation = await repository.GetReservationAsync(
            context.Message.ReservationId,
            context.CancellationToken);

        if (reservation is null || reservation.Status != ReservationStatus.Pending)
            return;

        reservation.Release(false);
        reservation.TicketType.Release(reservation.Quantity);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
