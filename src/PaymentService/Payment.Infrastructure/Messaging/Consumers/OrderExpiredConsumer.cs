using BuildingBlocks.Contracts.Orders;
using MassTransit;
using Payment.Application.Contracts;

namespace Payment.Infrastructure.Messaging.Consumers;

public sealed class OrderExpiredConsumer(
    IPaymentRepository repository,
    IAsaasGateway asaasGateway)
    : IConsumer<OrderExpiredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderExpiredIntegrationEvent> context)
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

        payment.Expire(DateTime.Now);
        await repository.SaveChangesAsync(context.CancellationToken);
    }
}
