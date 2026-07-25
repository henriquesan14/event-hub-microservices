using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Commands.ReleaseReservation;

public sealed record ReleaseReservationCommand(Guid Id) : ICommand<ResultT<ReservationDto>>;
