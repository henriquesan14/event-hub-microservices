using BuildingBlocks.Contracts.Payments;
using MassTransit;
using Ticketing.Application.Contracts;
using Ticketing.Domain.Enums;

namespace Ticketing.Infrastructure.Messaging.Consumers;

public sealed class PaymentApprovedConsumer(ITicketingRepository repository)
    : IConsumer<PaymentApprovedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentApprovedIntegrationEvent> context)
    {
        var message = context.Message;
        var reservation = await repository.GetReservationAsync(
            message.ReservationId,
            context.CancellationToken);

        if (reservation is null || reservation.Status == ReservationStatus.Confirmed)
            return;
        if (reservation.Status != ReservationStatus.Pending)
            return;

        reservation.Confirm(message.ApprovedAt);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
