using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Errors;

namespace Ticketing.Application.Queries.GetReservation;

public sealed class GetReservationQueryHandler(
    ITicketingRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetReservationQuery, ResultT<ReservationDetailsDto>>
{
    public async Task<ResultT<ReservationDetailsDto>> Handle(GetReservationQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return TicketingErrors.Unauthorized();
        var reservation = await repository.GetReservationAsync(request.Id, ct);
        if (reservation is null) return TicketingErrors.ReservationNotFound(request.Id);
        if (reservation.UserId != userId) return TicketingErrors.Forbidden();

        return new ReservationDetailsDto(
            reservation.Id,
            reservation.UserId,
            reservation.TicketTypeId,
            reservation.TicketType.EventId,
            reservation.TicketType.Name,
            reservation.TicketType.Price,
            reservation.TicketType.Currency,
            reservation.Quantity,
            reservation.ExpiresAt,
            reservation.Status);
    }
}
