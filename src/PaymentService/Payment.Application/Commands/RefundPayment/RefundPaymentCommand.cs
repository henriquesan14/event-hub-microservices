using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Commands.RefundPayment;

public sealed record RefundPaymentCommand(Guid PaymentId, string? Reason)
    : ICommand<ResultT<PaymentDto>>;
