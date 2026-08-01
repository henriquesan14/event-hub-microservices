using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Queries.GetPayments;

public sealed record GetPaymentsQuery : IQuery<ResultT<IReadOnlyList<PaymentDto>>>;
