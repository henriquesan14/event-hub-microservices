using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.Contracts.Ticketing;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Errors;
using Ticketing.Application.Extensions;
using Ticketing.Domain.Entities;
using MassTransit;

namespace Ticketing.Application.Commands.CreateReservation;

public sealed class CreateReservationCommandHandler(
    ITicketingRepository repository,
    IUserContext userContext,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<CreateReservationCommand, ResultT<ReservationDto>>
{
    public async Task<ResultT<ReservationDto>> Handle(CreateReservationCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return TicketingErrors.Unauthorized();

        var ticketType = await repository.GetTicketTypeAsync(request.TicketTypeId, ct);
        if (ticketType is null) return TicketingErrors.TicketTypeNotFound(request.TicketTypeId);

        var now = DateTime.Now;
        ticketType.EnsureEventSnapshot(request.EventName, request.EventStartsAt);
        ticketType.Reserve(request.Quantity, now);
        var reservation = TicketReservation.Create(
            ticketType.Id, userId, request.Quantity, now.AddMinutes(15));
        await repository.AddReservationAsync(reservation, ct);

        await publishEndpoint.Publish(
            new ReservationCreatedIntegrationEvent(
                reservation.Id,
                reservation.Id,
                userId,
                userContext.Name,
                ticketType.Id,
                ticketType.EventId,
                ticketType.EventName,
                ticketType.EventStartsAt!.Value,
                ticketType.Name,
                ticketType.Price,
                ticketType.Currency,
                reservation.Quantity,
                reservation.ExpiresAt),
            context => context.CorrelationId = reservation.Id,
            ct);

        await repository.SaveChangesAsync(ct);
        return reservation.ToDto();
    }
}
