using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;
using Order.Application.Dtos;
using Order.Application.Errors;
using Order.Application.Extensions;

namespace Order.Application.Queries.GetOrderByReservation;

public sealed class GetOrderByReservationQueryHandler(
    IOrderRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetOrderByReservationQuery, ResultT<OrderDto>>
{
    public async Task<ResultT<OrderDto>> Handle(GetOrderByReservationQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return OrderErrors.Unauthorized();
        var order = await repository.GetByReservationIdAsync(request.ReservationId, ct);
        if (order is null) return OrderErrors.NotFound(request.ReservationId);
        return order.UserId != userId ? OrderErrors.Forbidden() : order.ToDto();
    }
}
