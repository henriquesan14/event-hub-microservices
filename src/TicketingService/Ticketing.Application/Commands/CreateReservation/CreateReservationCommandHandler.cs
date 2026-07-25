using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Errors;
using Ticketing.Application.Extensions;
using Ticketing.Domain.Entities;

namespace Ticketing.Application.Commands.CreateReservation;

public sealed class CreateReservationCommandHandler(
    ITicketingRepository repository,
    IUserContext userContext)
    : ICommandHandler<CreateReservationCommand, ResultT<ReservationDto>>
{
    public async Task<ResultT<ReservationDto>> Handle(CreateReservationCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return TicketingErrors.Unauthorized();

        var ticketType = await repository.GetTicketTypeAsync(request.TicketTypeId, ct);
        if (ticketType is null) return TicketingErrors.TicketTypeNotFound(request.TicketTypeId);

        var now = DateTime.Now;
        ticketType.Reserve(request.Quantity, now);
        var reservation = TicketReservation.Create(
            ticketType.Id, userId, request.Quantity, now.AddMinutes(15));
        await repository.AddReservationAsync(reservation, ct);
        await repository.SaveChangesAsync(ct);
        return reservation.ToDto();
    }
}
