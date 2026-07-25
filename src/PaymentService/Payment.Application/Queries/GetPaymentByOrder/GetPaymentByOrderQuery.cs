using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Dtos;

namespace Payment.Application.Queries.GetPaymentByOrder;

public sealed record GetPaymentByOrderQuery(Guid OrderId) : IQuery<ResultT<PaymentDto>>;
