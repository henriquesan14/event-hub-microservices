using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Errors;
using Ticketing.Application.Extensions;

namespace Ticketing.Application.Commands.ReleaseReservation;

public sealed class ReleaseReservationCommandHandler(
    ITicketingRepository repository,
    IUserContext userContext)
    : ICommandHandler<ReleaseReservationCommand, ResultT<ReservationDto>>
{
    public async Task<ResultT<ReservationDto>> Handle(ReleaseReservationCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return TicketingErrors.Unauthorized();
        var reservation = await repository.GetReservationAsync(request.Id, ct);
        if (reservation is null) return TicketingErrors.ReservationNotFound(request.Id);
        if (reservation.UserId != userId) return TicketingErrors.Forbidden();

        reservation.Release(false);
        reservation.TicketType.Release(reservation.Quantity);
        await repository.SaveChangesAsync(ct);
        return reservation.ToDto();
    }
}
