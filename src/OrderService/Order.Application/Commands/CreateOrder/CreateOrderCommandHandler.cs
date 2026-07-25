using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;
using Order.Application.Dtos;
using Order.Application.Errors;
using Order.Application.Extensions;

namespace Order.Application.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository repository,
    ITicketingGateway ticketingGateway,
    IUserContext userContext)
    : ICommandHandler<CreateOrderCommand, ResultT<OrderDto>>
{
    public async Task<ResultT<OrderDto>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return OrderErrors.Unauthorized();
        if (await repository.ReservationHasOrderAsync(request.ReservationId, ct))
            return OrderErrors.ReservationAlreadyUsed();

        var reservation = await ticketingGateway.GetReservationAsync(request.ReservationId, ct);
        if (reservation is null ||
            reservation.UserId != userId ||
            !string.Equals(reservation.Status, "Pending", StringComparison.OrdinalIgnoreCase) ||
            reservation.ExpiresAt <= DateTime.Now)
            return OrderErrors.ReservationInvalid();

        var order = Domain.Entities.Order.Create(
            userId,
            reservation.Id,
            reservation.TicketTypeId,
            reservation.EventId,
            reservation.TicketName,
            reservation.UnitPrice,
            reservation.Currency,
            reservation.Quantity,
            reservation.ExpiresAt);

        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);
        return order.ToDto();
    }
}
