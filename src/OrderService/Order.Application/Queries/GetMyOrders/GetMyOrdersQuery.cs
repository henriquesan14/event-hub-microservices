using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Dtos;

namespace Order.Application.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery : IQuery<ResultT<IReadOnlyList<OrderDto>>>;
