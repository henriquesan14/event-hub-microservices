using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Commands.ApprovePayment;

public sealed record ApprovePaymentCommand(
    Guid Id,
    string ProviderReference) : ICommand<ResultT<PaymentDto>>;
