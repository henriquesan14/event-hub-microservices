using BuildingBlocks.Contracts.Payments;
using MassTransit;
using Ticketing.Application.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.Infrastructure.Messaging.Consumers;

public sealed class PaymentRefundedConsumer(ITicketingRepository repository)
    : IConsumer<PaymentRefundedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentRefundedIntegrationEvent> context)
    {
        var reservation = await repository.GetReservationAsync(
            context.Message.ReservationId,
            context.CancellationToken);
        if (reservation is null || reservation.Status == ReservationStatus.Refunded) return;
        if (reservation.Status != ReservationStatus.Confirmed) return;

        reservation.Refund();
        reservation.TicketType.Release(reservation.Quantity);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
