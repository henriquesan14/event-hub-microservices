using BuildingBlocks.Contracts.Orders;
using MassTransit;
using Payment.Application.Contracts;

namespace Payment.Infrastructure.Messaging.Consumers;

public sealed class OrderCreatedConsumer(IPaymentRepository repository)
    : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        if (await repository.OrderHasPaymentAsync(message.OrderId, context.CancellationToken))
            return;

        var payment = Domain.Entities.Payment.Create(
            message.OrderId,
            message.ReservationId,
            message.UserId,
            message.Total,
            message.Currency,
            message.ExpiresAt);

        payment.CreatedBy = message.UserId;
        await repository.AddAsync(payment, context.CancellationToken);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
