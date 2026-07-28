using Admission.Application.Commands.CheckInTicket;
using Admission.Application.Queries.GetMyTickets;
using Admission.Application.Queries.GetTicket;
using BuildingBlocks.Api.Extensions;
using Carter;
using FluentValidation;
using MediatR;

namespace Admission.Api.Endpoints;

public sealed class AdmissionModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admission").RequireAuthorization();
        group.MapGet("/tickets/me", GetMine);
        group.MapGet("/tickets/{id:guid}", GetById);
        group.MapPost("/check-in", CheckIn)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> GetMine(ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetMyTicketsQuery(), ct)).ToHttpResult();

    private static async Task<IResult> GetById(
        Guid id,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new GetTicketQuery(id), ct)).ToHttpResult();

    private static async Task<IResult> CheckIn(
        CheckInTicketCommand command,
        IValidator<CheckInTicketCommand> validator,
        ISender sender,
        CancellationToken ct)
    {
        var validation = await validator.ValidateRequest(command, ct);
        return validation ?? (await sender.Send(command, ct)).ToHttpResult();
    }
}
