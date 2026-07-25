using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Errors;
using Ticketing.Application.Extensions;

namespace Ticketing.Application.Commands.ConfirmReservation;

public sealed class ConfirmReservationCommandHandler(
    ITicketingRepository repository,
    IUserContext userContext)
    : ICommandHandler<ConfirmReservationCommand, ResultT<ReservationDto>>
{
    public async Task<ResultT<ReservationDto>> Handle(ConfirmReservationCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return TicketingErrors.Unauthorized();
        var reservation = await repository.GetReservationAsync(request.Id, ct);
        if (reservation is null) return TicketingErrors.ReservationNotFound(request.Id);
        if (reservation.UserId != userId) return TicketingErrors.Forbidden();

        reservation.Confirm(DateTime.Now);
        await repository.SaveChangesAsync(ct);
        return reservation.ToDto();
    }
}
