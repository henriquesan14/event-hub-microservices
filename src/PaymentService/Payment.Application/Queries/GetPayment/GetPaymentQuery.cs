using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Queries.GetPayment;

public sealed record GetPaymentQuery(Guid Id) : IQuery<ResultT<PaymentDto>>;
