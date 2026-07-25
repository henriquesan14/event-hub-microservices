using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Contracts;
using Payment.Application.Dtos;
using Payment.Application.Errors;
using Payment.Application.Extensions;

namespace Payment.Application.Queries.GetMyPayments;

public sealed class GetMyPaymentsQueryHandler(
    IPaymentRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetMyPaymentsQuery, ResultT<IReadOnlyList<PaymentDto>>>
{
    public async Task<ResultT<IReadOnlyList<PaymentDto>>> Handle(
        GetMyPaymentsQuery request,
        CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return PaymentErrors.Unauthorized();
        var payments = await repository.GetByUserAsync(userId, ct);
        return payments.Select(x => x.ToDto()).ToList();
    }
}
