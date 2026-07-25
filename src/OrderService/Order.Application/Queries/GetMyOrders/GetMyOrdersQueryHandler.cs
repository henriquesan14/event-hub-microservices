using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;
using Order.Application.Dtos;
using Order.Application.Errors;
using Order.Application.Extensions;

namespace Order.Application.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler(IOrderRepository repository, IUserContext userContext)
    : IQueryHandler<GetMyOrdersQuery, ResultT<IReadOnlyList<OrderDto>>>
{
    public async Task<ResultT<IReadOnlyList<OrderDto>>> Handle(GetMyOrdersQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return OrderErrors.Unauthorized();
        var orders = await repository.GetByUserAsync(userId, ct);
        return orders.Select(x => x.ToDto()).ToList();
    }
}
