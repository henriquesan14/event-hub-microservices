using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;
using Order.Application.Errors;

namespace Order.Application.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(
    IOrderRepository repository,
    ITicketingGateway ticketingGateway,
    IUserContext userContext)
    : ICommandHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return OrderErrors.Unauthorized();
        var order = await repository.GetByIdAsync(request.Id, ct);
        if (order is null) return OrderErrors.NotFound(request.Id);
        if (order.UserId != userId) return OrderErrors.Forbidden();

        if (!await ticketingGateway.ReleaseReservationAsync(order.ReservationId, ct))
            return OrderErrors.TicketingUnavailable();

        order.Cancel();
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
