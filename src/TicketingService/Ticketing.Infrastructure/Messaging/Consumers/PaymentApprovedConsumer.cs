using BuildingBlocks.Contracts.Payments;
using MassTransit;
using Ticketing.Application.Contracts;
using Ticketing.Domain.Enums;
using BuildingBlocks.Contracts.Tickets;

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
        await context.Publish(
            new ReservationConfirmedIntegrationEvent(
                message.CorrelationId,
                message.PaymentId,
                message.OrderId,
                reservation.Id,
                reservation.UserId,
                reservation.TicketType.EventId,
                reservation.TicketType.EventName,
                reservation.TicketType.EventStartsAt!.Value,
                reservation.TicketTypeId,
                reservation.TicketType.Name,
                reservation.Quantity,
                message.ApprovedAt),
            publish => publish.CorrelationId = message.CorrelationId);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
