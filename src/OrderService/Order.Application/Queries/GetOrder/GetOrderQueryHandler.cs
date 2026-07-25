using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Contracts;
using Order.Application.Dtos;
using Order.Application.Errors;
using Order.Application.Extensions;

namespace Order.Application.Queries.GetOrder;

public sealed class GetOrderQueryHandler(IOrderRepository repository, IUserContext userContext)
    : IQueryHandler<GetOrderQuery, ResultT<OrderDto>>
{
    public async Task<ResultT<OrderDto>> Handle(GetOrderQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return OrderErrors.Unauthorized();
        var order = await repository.GetByIdAsync(request.Id, ct);
        if (order is null) return OrderErrors.NotFound(request.Id);
        return order.UserId != userId ? OrderErrors.Forbidden() : order.ToDto();
    }
}
