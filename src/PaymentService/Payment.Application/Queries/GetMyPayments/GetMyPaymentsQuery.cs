using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Queries.GetMyPayments;

public sealed record GetMyPaymentsQuery : IQuery<ResultT<IReadOnlyList<PaymentDto>>>;
