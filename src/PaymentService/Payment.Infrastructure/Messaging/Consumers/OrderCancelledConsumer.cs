using BuildingBlocks.Contracts.Orders;
using MassTransit;
using Payment.Application.Contracts;

namespace Payment.Infrastructure.Messaging.Consumers;

public sealed class OrderCancelledConsumer(
    IPaymentRepository repository,
    IAsaasGateway asaasGateway)
    : IConsumer<OrderCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledIntegrationEvent> context)
    {
        var payment = await repository.GetByOrderIdAsync(
            context.Message.OrderId,
            context.CancellationToken);
        if (payment is null)
            return;

        if (!string.IsNullOrWhiteSpace(payment.ProviderReference))
            await asaasGateway.CancelChargeAsync(
                payment.ProviderReference,
                context.CancellationToken);

        payment.Cancel(DateTime.Now);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
