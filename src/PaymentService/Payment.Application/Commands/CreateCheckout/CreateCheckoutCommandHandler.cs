using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Contracts;
using Payment.Application.Dtos;
using Payment.Application.Errors;
using Payment.Application.Extensions;
using Payment.Domain.Enums;

namespace Payment.Application.Commands.CreateCheckout;

public sealed class CreateCheckoutCommandHandler(
    IPaymentRepository repository,
    IAsaasGateway asaasGateway,
    IUserContext userContext)
    : ICommandHandler<CreateCheckoutCommand, ResultT<PaymentDto>>
{
    private static readonly HashSet<string> BillingTypes =
        ["UNDEFINED", "PIX", "BOLETO", "CREDIT_CARD"];

    public async Task<ResultT<PaymentDto>> Handle(CreateCheckoutCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return PaymentErrors.Unauthorized();
        var payment = await repository.GetByIdAsync(request.PaymentId, ct);
        if (payment is null) return PaymentErrors.NotFound(request.PaymentId);
        if (payment.UserId != userId) return PaymentErrors.Forbidden();
        if (payment.Status != PaymentStatus.Pending)
            return PaymentErrors.InvalidState(payment.Status.ToString());
        if (!string.Equals(payment.Currency, "BRL", StringComparison.OrdinalIgnoreCase))
            return PaymentErrors.UnsupportedCurrency(payment.Currency);
        if (!string.IsNullOrWhiteSpace(payment.ProviderReference))
            return payment.ToDto();
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.CpfCnpj))
            return PaymentErrors.InvalidCheckoutData();
        if (string.IsNullOrWhiteSpace(request.BillingType))
            return PaymentErrors.InvalidBillingType();

        var billingType = request.BillingType.Trim().ToUpperInvariant();
        if (!BillingTypes.Contains(billingType))
            return PaymentErrors.InvalidBillingType();

        var charge = await asaasGateway.CreateChargeAsync(
            new CreateAsaasCharge(
                payment.Id,
                payment.UserId,
                request.Name,
                request.Email,
                request.CpfCnpj,
                request.MobilePhone,
                billingType,
                payment.Amount,
                payment.ExpiresAt,
                $"EventHub order {payment.OrderId}"),
            ct);

        payment.AttachProviderCharge(
            charge.PaymentId,
            charge.CustomerId,
            charge.BillingType,
            charge.InvoiceUrl);
        await repository.SaveChangesAsync(ct);
        return payment.ToDto();
    }
}
