using BuildingBlocks.Api.Extensions;
using Carter;
using MediatR;
using Payment.Application.Commands.CreateCheckout;
using Payment.Application.Commands.RefundPayment;
using Payment.Application.Queries.GetMyPayments;
using Payment.Application.Queries.GetPayment;
using Payment.Application.Queries.GetPaymentByOrder;
using Payment.Application.Queries.GetPayments;

namespace Payment.Api.Endpoints;

public sealed class PaymentModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments").RequireAuthorization();
        group.MapGet("/me", GetMine);
        group.MapGet("", GetAll)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
        group.MapGet("/by-order/{orderId:guid}", GetByOrder);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/{id:guid}/checkout", Checkout);
        group.MapPost("/{id:guid}/refund", Refund)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> GetMine(ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetMyPaymentsQuery(), ct)).ToHttpResult();

    private static async Task<IResult> GetAll(ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetPaymentsQuery(), ct)).ToHttpResult();

    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetPaymentQuery(id), ct)).ToHttpResult();

    private static async Task<IResult> GetByOrder(
        Guid orderId,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new GetPaymentByOrderQuery(orderId), ct)).ToHttpResult();

    private static async Task<IResult> Checkout(
        Guid id,
        CreateCheckoutRequest request,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(
            new CreateCheckoutCommand(
                id,
                request.Name,
                request.Email,
                request.CpfCnpj,
                request.MobilePhone,
                request.BillingType),
            ct))
        .ToHttpResult();

    private sealed record CreateCheckoutRequest(
        string Name,
        string Email,
        string CpfCnpj,
        string? MobilePhone,
        string BillingType = "UNDEFINED");

    private static async Task<IResult> Refund(
        Guid id,
        RefundPaymentRequest request,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new RefundPaymentCommand(id, request.Reason), ct)).ToHttpResult();

    private sealed record RefundPaymentRequest(string? Reason);
}
