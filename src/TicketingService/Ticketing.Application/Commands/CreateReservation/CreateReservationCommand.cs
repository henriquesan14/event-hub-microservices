using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Commands.CreateReservation;

public sealed record CreateReservationCommand(
    Guid TicketTypeId,
    string EventName,
    DateTime EventStartsAt,
    int Quantity)
    : ICommand<ResultT<ReservationDto>>;
