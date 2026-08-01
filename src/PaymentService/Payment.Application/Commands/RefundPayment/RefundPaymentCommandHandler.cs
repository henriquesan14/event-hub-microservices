using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Contracts;
using Payment.Application.Dtos;
using Payment.Application.Errors;
using Payment.Application.Extensions;
using Payment.Domain.Enums;

namespace Payment.Application.Commands.RefundPayment;

public sealed class RefundPaymentCommandHandler(
    IPaymentRepository repository,
    IAsaasGateway asaasGateway)
    : ICommandHandler<RefundPaymentCommand, ResultT<PaymentDto>>
{
    public async Task<ResultT<PaymentDto>> Handle(RefundPaymentCommand request, CancellationToken ct)
    {
        var payment = await repository.GetByIdAsync(request.PaymentId, ct);
        if (payment is null) return PaymentErrors.NotFound(request.PaymentId);
        if (payment.Status is PaymentStatus.RefundPending or PaymentStatus.Refunded)
            return payment.ToDto();
        if (payment.Status != PaymentStatus.Approved)
            return PaymentErrors.InvalidState(payment.Status.ToString());
        if (string.IsNullOrWhiteSpace(payment.ProviderReference))
            return PaymentErrors.ProviderReferenceRequired();
        if (string.Equals(payment.BillingType, "BOLETO", StringComparison.OrdinalIgnoreCase))
            return PaymentErrors.BankSlipRefundUnsupported();

        await asaasGateway.RefundChargeAsync(payment.ProviderReference, request.Reason, ct);
        payment.RequestRefund(request.Reason, DateTime.Now);
        await repository.SaveChangesAsync(ct);
        return payment.ToDto();
    }
}
