using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;
using Order.Application.Errors;
using BuildingBlocks.Contracts.Orders;
using MassTransit;

namespace Order.Application.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(
    IOrderRepository repository,
    IUserContext userContext,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return OrderErrors.Unauthorized();
        var order = await repository.GetByIdAsync(request.Id, ct);
        if (order is null) return OrderErrors.NotFound(request.Id);
        if (order.UserId != userId) return OrderErrors.Forbidden();

        order.Cancel();
        await publishEndpoint.Publish(
            new OrderCancelledIntegrationEvent(
                order.ReservationId, order.Id, order.ReservationId, order.UserId),
            context => context.CorrelationId = order.ReservationId,
            ct);
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
