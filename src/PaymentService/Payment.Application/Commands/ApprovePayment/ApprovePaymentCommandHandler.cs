using BuildingBlocks.Contracts.Payments;
using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using MassTransit;
using Payment.Application.Contracts;
using Payment.Application.Dtos;
using Payment.Application.Errors;
using Payment.Application.Extensions;
using Payment.Domain.Enums;

namespace Payment.Application.Commands.ApprovePayment;

public sealed class ApprovePaymentCommandHandler(
    IPaymentRepository repository,
    IUserContext userContext,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<ApprovePaymentCommand, ResultT<PaymentDto>>
{
    public async Task<ResultT<PaymentDto>> Handle(ApprovePaymentCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return PaymentErrors.Unauthorized();
        var payment = await repository.GetByIdAsync(request.Id, ct);
        if (payment is null) return PaymentErrors.NotFound(request.Id);
        if (payment.UserId != userId) return PaymentErrors.Forbidden();
        if (payment.Status == PaymentStatus.Approved) return payment.ToDto();
        if (payment.Status != PaymentStatus.Pending)
            return PaymentErrors.InvalidState(payment.Status.ToString());

        var approvedAt = DateTime.Now;
        payment.Approve(request.ProviderReference, approvedAt);
        await publishEndpoint.Publish(
            new PaymentApprovedIntegrationEvent(
                payment.OrderId,
                payment.Id,
                payment.OrderId,
                payment.ReservationId,
                payment.UserId,
                payment.Amount,
                payment.Currency,
                approvedAt),
            context => context.CorrelationId = payment.OrderId,
            ct);
        await repository.SaveChangesAsync(ct);
        return payment.ToDto();
    }
}
