using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Commands.ConfirmReservation;

public sealed record ConfirmReservationCommand(Guid Id) : ICommand<ResultT<ReservationDto>>;
