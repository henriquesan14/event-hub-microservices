using BuildingBlocks.Contracts.Payments;
using MassTransit;
using Order.Application.Contracts;
using Order.Domain.Enums;

namespace Order.Infrastructure.Messaging.Consumers;

public sealed class PaymentApprovedConsumer(IOrderRepository repository)
    : IConsumer<PaymentApprovedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentApprovedIntegrationEvent> context)
    {
        var message = context.Message;
        var order = await repository.GetByIdAsync(message.OrderId, context.CancellationToken);
        if (order is null || order.Status == OrderStatus.Paid)
            return;
        if (order.Status != OrderStatus.PendingPayment)
            return;

        order.ConfirmPayment(message.PaymentId);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
