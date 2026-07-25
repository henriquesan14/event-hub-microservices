using BuildingBlocks.Api.Extensions;
using Carter;
using FluentValidation;
using MediatR;
using Ticketing.Application.Commands.ConfirmReservation;
using Ticketing.Application.Commands.CreateReservation;
using Ticketing.Application.Commands.CreateTicketType;
using Ticketing.Application.Commands.DeleteTicketType;
using Ticketing.Application.Commands.ExpireReservations;
using Ticketing.Application.Commands.ReleaseReservation;
using Ticketing.Application.Commands.UpdateTicketType;
using Ticketing.Application.Queries.GetTicketType;
using Ticketing.Application.Queries.GetTicketTypesByEvent;
using Ticketing.Application.Queries.GetReservation;

namespace Ticketing.Api.Endpoints;

public sealed class TicketingModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var tickets = app.MapGroup("/api");
        tickets.MapGet("/events/{eventId:guid}/tickets", GetByEvent);
        tickets.MapGet("/tickets/{id:guid}", GetById);
        tickets.MapPost("/events/{eventId:guid}/tickets", Create).RequireAuthorization();
        tickets.MapPut("/tickets/{id:guid}", Update).RequireAuthorization();
        tickets.MapDelete("/tickets/{id:guid}", Delete).RequireAuthorization();

        tickets.MapPost("/tickets/{ticketTypeId:guid}/reservations", Reserve).RequireAuthorization();
        tickets.MapGet("/reservations/{id:guid}", GetReservation).RequireAuthorization();
        tickets.MapPost("/reservations/{id:guid}/confirm", Confirm).RequireAuthorization();
        tickets.MapDelete("/reservations/{id:guid}", Release).RequireAuthorization();
        tickets.MapPost("/reservations/expire", Expire).RequireAuthorization();
    }

    private static async Task<IResult> GetByEvent(Guid eventId, ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetTicketTypesByEventQuery(eventId), ct)).ToHttpResult();

    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetTicketTypeQuery(id), ct)).ToHttpResult();

    private static async Task<IResult> Create(
        Guid eventId,
        CreateTicketTypeRequest request,
        IValidator<CreateTicketTypeCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var command = new CreateTicketTypeCommand(
            eventId, request.Name, request.Description, request.Price, request.Currency,
            request.TotalQuantity, request.SalesStart, request.SalesEnd);
        var validation = await validator.ValidateRequest(command, ct);
        return validation ?? (await sender.Send(command, ct)).ToHttpResult();
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateTicketTypeRequest request,
        IValidator<UpdateTicketTypeCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateTicketTypeCommand(
            id, request.Name, request.Description, request.Price, request.Currency,
            request.TotalQuantity, request.SalesStart, request.SalesEnd, request.Active);
        var validation = await validator.ValidateRequest(command, ct);
        return validation ?? (await sender.Send(command, ct)).ToHttpResult();
    }

    private static async Task<IResult> Delete(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new DeleteTicketTypeCommand(id), ct)).ToHttpResult();

    private static async Task<IResult> Reserve(
        Guid ticketTypeId,
        CreateReservationRequest request,
        IValidator<CreateReservationCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var command = new CreateReservationCommand(ticketTypeId, request.Quantity);
        var validation = await validator.ValidateRequest(command, ct);
        return validation ?? (await sender.Send(command, ct)).ToHttpResult();
    }

    private static async Task<IResult> Confirm(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new ConfirmReservationCommand(id), ct)).ToHttpResult();

    private static async Task<IResult> Release(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new ReleaseReservationCommand(id), ct)).ToHttpResult();

    private static async Task<IResult> Expire(ISender sender, CancellationToken ct) =>
        (await sender.Send(new ExpireReservationsCommand(), ct)).ToHttpResult();

    private static async Task<IResult> GetReservation(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetReservationQuery(id), ct)).ToHttpResult();
}

public sealed record CreateTicketTypeRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int TotalQuantity,
    DateTime SalesStart,
    DateTime SalesEnd);

public sealed record UpdateTicketTypeRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int TotalQuantity,
    DateTime SalesStart,
    DateTime SalesEnd,
    bool Active);

public sealed record CreateReservationRequest(int Quantity);
