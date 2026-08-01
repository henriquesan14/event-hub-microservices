using BuildingBlocks.Contracts.Payments;
using MassTransit;
using Order.Application.Contracts;
using Order.Domain.Enums;

namespace Order.Infrastructure.Messaging.Consumers;

public sealed class PaymentRefundedConsumer(IOrderRepository repository)
    : IConsumer<PaymentRefundedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentRefundedIntegrationEvent> context)
    {
        var message = context.Message;
        var order = await repository.GetByIdAsync(message.OrderId, context.CancellationToken);
        if (order is null || order.Status == OrderStatus.Refunded) return;
        if (order.Status != OrderStatus.Paid) return;
        order.Refund(message.PaymentId);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
