using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Commands.FailPayment;

public sealed record FailPaymentCommand(
    Guid Id,
    string Reason) : ICommand<ResultT<PaymentDto>>;
