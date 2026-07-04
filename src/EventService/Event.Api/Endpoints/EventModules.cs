using BuildingBlocks.Api.Extensions;
using Carter;
using Events.Application.Queries.GetEvents;
using EventsApplication.Commands.CreateEvent;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Events.Api.Endpoints;

public sealed class EventModules : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events");

        group.MapPost("/", Create);
        group.MapGet("/", GetEvents);
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
}
