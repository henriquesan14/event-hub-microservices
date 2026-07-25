using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Dtos;

namespace Order.Application.Queries.GetOrder;

public sealed record GetOrderQuery(Guid Id) : IQuery<ResultT<OrderDto>>;
