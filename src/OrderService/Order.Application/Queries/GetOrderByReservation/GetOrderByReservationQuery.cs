using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Dtos;

namespace Order.Application.Queries.GetOrderByReservation;

public sealed record GetOrderByReservationQuery(Guid ReservationId) : IQuery<ResultT<OrderDto>>;
