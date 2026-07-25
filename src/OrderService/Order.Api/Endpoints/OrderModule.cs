using BuildingBlocks.Api.Extensions;
using Carter;
using MediatR;
using Order.Application.Commands.CancelOrder;
using Order.Application.Commands.CreateOrder;
using Order.Application.Commands.ExpireOrders;
using Order.Application.Queries.GetMyOrders;
using Order.Application.Queries.GetOrder;

namespace Order.Api.Endpoints;

public sealed class OrderModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").RequireAuthorization();
        group.MapPost("/", Create);
        group.MapGet("/me", GetMine);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/{id:guid}/cancel", Cancel);
        group.MapPost("/expire", Expire)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> Create(
        CreateOrderRequest request,
        ISender sender,
        CancellationToken ct)
    {
        if (request.ReservationId == Guid.Empty)
            return Results.BadRequest(new { message = "ReservationId is required." });
        return (await sender.Send(new CreateOrderCommand(request.ReservationId), ct)).ToHttpResult();
    }

    private static async Task<IResult> GetMine(ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetMyOrdersQuery(), ct)).ToHttpResult();

    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetOrderQuery(id), ct)).ToHttpResult();

    private static async Task<IResult> Cancel(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new CancelOrderCommand(id), ct)).ToHttpResult();

    private static async Task<IResult> Expire(ISender sender, CancellationToken ct) =>
        (await sender.Send(new ExpireOrdersCommand(), ct)).ToHttpResult();
}

public sealed record CreateOrderRequest(Guid ReservationId);
