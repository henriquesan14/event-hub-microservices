using BuildingBlocks.Api.Extensions;
using Carter;
using MediatR;
using Payment.Application.Commands.ApprovePayment;
using Payment.Application.Commands.FailPayment;
using Payment.Application.Queries.GetMyPayments;
using Payment.Application.Queries.GetPayment;
using Payment.Application.Queries.GetPaymentByOrder;

namespace Payment.Api.Endpoints;

public sealed class PaymentModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments").RequireAuthorization();
        group.MapGet("/me", GetMine);
        group.MapGet("/by-order/{orderId:guid}", GetByOrder);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/{id:guid}/approve", Approve);
        group.MapPost("/{id:guid}/fail", Fail);
    }

    private static async Task<IResult> GetMine(ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetMyPaymentsQuery(), ct)).ToHttpResult();

    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetPaymentQuery(id), ct)).ToHttpResult();

    private static async Task<IResult> GetByOrder(
        Guid orderId,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new GetPaymentByOrderQuery(orderId), ct)).ToHttpResult();

    private static async Task<IResult> Approve(
        Guid id,
        ApprovePaymentRequest request,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new ApprovePaymentCommand(id, request.ProviderReference), ct))
        .ToHttpResult();

    private static async Task<IResult> Fail(
        Guid id,
        FailPaymentRequest request,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new FailPaymentCommand(id, request.Reason), ct))
        .ToHttpResult();

    private sealed record ApprovePaymentRequest(string ProviderReference);
    private sealed record FailPaymentRequest(string Reason);
}
