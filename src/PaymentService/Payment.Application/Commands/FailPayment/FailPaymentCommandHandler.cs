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

namespace Payment.Application.Commands.FailPayment;

public sealed class FailPaymentCommandHandler(
    IPaymentRepository repository,
    IUserContext userContext,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<FailPaymentCommand, ResultT<PaymentDto>>
{
    public async Task<ResultT<PaymentDto>> Handle(FailPaymentCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return PaymentErrors.Unauthorized();
        var payment = await repository.GetByIdAsync(request.Id, ct);
        if (payment is null) return PaymentErrors.NotFound(request.Id);
        if (payment.UserId != userId) return PaymentErrors.Forbidden();
        if (payment.Status == PaymentStatus.Failed) return payment.ToDto();
        if (payment.Status != PaymentStatus.Pending)
            return PaymentErrors.InvalidState(payment.Status.ToString());

        var failedAt = DateTime.Now;
        payment.Fail(request.Reason, failedAt);
        await publishEndpoint.Publish(
            new PaymentFailedIntegrationEvent(
                payment.OrderId,
                payment.Id,
                payment.OrderId,
                payment.ReservationId,
                payment.UserId,
                payment.FailureReason!,
                failedAt),
            context => context.CorrelationId = payment.OrderId,
            ct);
        await repository.SaveChangesAsync(ct);
        return payment.ToDto();
    }
}
