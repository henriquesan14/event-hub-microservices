using BuildingBlocks.Contracts.Orders;
using MassTransit;
using Payment.Application.Contracts;

namespace Payment.Infrastructure.Messaging.Consumers;

public sealed class OrderExpiredConsumer(IPaymentRepository repository)
    : IConsumer<OrderExpiredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderExpiredIntegrationEvent> context)
    {
        var payment = await repository.GetByOrderIdAsync(
            context.Message.OrderId,
            context.CancellationToken);
        if (payment is null)
            return;

        payment.Expire(DateTime.Now);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
