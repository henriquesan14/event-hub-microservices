using BuildingBlocks.Api.Extensions;
using Carter;
using EventsApplication.Commands.CreateEvent;
using FluentValidation;
using MediatR;

namespace Events.Api.Endpoints;

public sealed class EventModules : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").RequireAuthorization();

        group.MapPost("/", Create);
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
}
