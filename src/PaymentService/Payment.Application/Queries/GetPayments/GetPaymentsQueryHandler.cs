using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Contracts;
using Payment.Application.Dtos;
using Payment.Application.Extensions;

namespace Payment.Application.Queries.GetPayments;

public sealed class GetPaymentsQueryHandler(IPaymentRepository repository)
    : IQueryHandler<GetPaymentsQuery, ResultT<IReadOnlyList<PaymentDto>>>
{
    public async Task<ResultT<IReadOnlyList<PaymentDto>>> Handle(
        GetPaymentsQuery request,
        CancellationToken ct) =>
        (await repository.GetAllAsync(ct)).Select(x => x.ToDto()).ToList();
}
