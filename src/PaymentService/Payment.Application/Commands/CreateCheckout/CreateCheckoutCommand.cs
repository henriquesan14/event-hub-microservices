using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Commands.CreateCheckout;

public sealed record CreateCheckoutCommand(
    Guid PaymentId,
    string Name,
    string Email,
    string CpfCnpj,
    string? MobilePhone,
    string BillingType) : ICommand<ResultT<PaymentDto>>;
