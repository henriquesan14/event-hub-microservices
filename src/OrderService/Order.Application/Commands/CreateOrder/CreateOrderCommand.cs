using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Order.Application.Dtos;

namespace Order.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid ReservationId) : ICommand<ResultT<OrderDto>>;
