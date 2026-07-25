using BuildingBlocks.Contracts.Orders;
using MassTransit;
using Payment.Application.Contracts;

namespace Payment.Infrastructure.Messaging.Consumers;

public sealed class OrderCancelledConsumer(IPaymentRepository repository)
    : IConsumer<OrderCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledIntegrationEvent> context)
    {
        var payment = await repository.GetByOrderIdAsync(
            context.Message.OrderId,
            context.CancellationToken);
        if (payment is null)
            return;

        payment.Cancel(DateTime.Now);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
