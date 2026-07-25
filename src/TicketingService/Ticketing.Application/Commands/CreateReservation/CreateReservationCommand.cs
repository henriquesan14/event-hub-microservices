using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Commands.CreateReservation;

public sealed record CreateReservationCommand(Guid TicketTypeId, int Quantity)
    : ICommand<ResultT<ReservationDto>>;
