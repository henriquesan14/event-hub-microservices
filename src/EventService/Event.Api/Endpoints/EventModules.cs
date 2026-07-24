using BuildingBlocks.Api.Extensions;
using Carter;
using Events.Application.Queries.GetEvents;
using Events.Application.Dtos;
using EventsApplication.Commands.CancelEvent;
using EventsApplication.Commands.CreateEvent;
using EventsApplication.Commands.DeleteEvent;
using EventsApplication.Commands.PublishEvent;
using EventsApplication.Commands.UpdateEvent;
using EventsApplication.Queries.GetEventById;
using FluentValidation;
using MediatR;

namespace Events.Api.Endpoints;

public sealed class EventModules : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").RequireAuthorization();

        group.MapPost("/", Create).RequireAuthorization();
        group.MapGet("/", GetEvents);
        group.MapGet("/{id:guid}", GetById);
        group.MapPut("/{id:guid}", Update).RequireAuthorization();
        group.MapPost("/{id:guid}/publish", Publish).RequireAuthorization();
        group.MapPost("/{id:guid}/cancel", Cancel).RequireAuthorization();
        group.MapDelete("/{id:guid}", Delete).RequireAuthorization();
    }

    private static async Task<IResult> Create(
        CreateEventCommand command,
        IValidator<CreateEventCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);

        if (validation is not null)
            return validation;

        var result = await sender.Send(command, ct);

        return result.ToHttpResult();
    }

    private static async Task<IResult> GetEvents(
        [AsParameters] GetEventsQuery query,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        return result.ToHttpResult();
    }

    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetEventByIdQuery(id), ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateEventRequest request,
        IValidator<UpdateEventCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateEventCommand(
            id, request.Title, request.Description, request.Address, request.StartsAt, request.EndsAt);
        var validation = await validator.ValidateRequest(command, ct);
        if (validation is not null)
            return validation;

        var result = await sender.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> Publish(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new PublishEventCommand(id), ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> Cancel(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new CancelEventCommand(id), ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> Delete(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteEventCommand(id), ct);
        return result.ToHttpResult();
    }
}

public sealed record UpdateEventRequest(
    string Title,
    string Description,
    AddressRequest Address,
    DateTime StartsAt,
    DateTime EndsAt);
