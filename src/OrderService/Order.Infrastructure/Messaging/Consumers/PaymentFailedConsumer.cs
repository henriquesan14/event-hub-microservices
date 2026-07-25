using BuildingBlocks.Contracts.Payments;
using MassTransit;
using Order.Application.Contracts;
using Order.Domain.Enums;

namespace Order.Infrastructure.Messaging.Consumers;

public sealed class PaymentFailedConsumer(IOrderRepository repository)
    : IConsumer<PaymentFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
    {
        var message = context.Message;
        var order = await repository.GetByIdAsync(message.OrderId, context.CancellationToken);
        if (order is null || order.Status == OrderStatus.PaymentFailed)
            return;
        if (order.Status != OrderStatus.PendingPayment)
            return;

        order.FailPayment(message.PaymentId);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
