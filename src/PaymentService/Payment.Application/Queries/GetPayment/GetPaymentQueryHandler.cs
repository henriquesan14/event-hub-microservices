using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Contracts;
using Payment.Application.Dtos;
using Payment.Application.Errors;
using Payment.Application.Extensions;

namespace Payment.Application.Queries.GetPayment;

public sealed class GetPaymentQueryHandler(IPaymentRepository repository, IUserContext userContext)
    : IQueryHandler<GetPaymentQuery, ResultT<PaymentDto>>
{
    public async Task<ResultT<PaymentDto>> Handle(GetPaymentQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return PaymentErrors.Unauthorized();
        var payment = await repository.GetByIdAsync(request.Id, ct);
        if (payment is null) return PaymentErrors.NotFound(request.Id);
        return payment.UserId != userId ? PaymentErrors.Forbidden() : payment.ToDto();
    }
}
