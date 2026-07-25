using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Payment.Application.Contracts;
using Payment.Application.Dtos;
using Payment.Application.Errors;
using Payment.Application.Extensions;

namespace Payment.Application.Queries.GetPaymentByOrder;

public sealed class GetPaymentByOrderQueryHandler(
    IPaymentRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetPaymentByOrderQuery, ResultT<PaymentDto>>
{
    public async Task<ResultT<PaymentDto>> Handle(GetPaymentByOrderQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return PaymentErrors.Unauthorized();
        var payment = await repository.GetByOrderIdAsync(request.OrderId, ct);
        if (payment is null) return PaymentErrors.OrderNotFound(request.OrderId);
        return payment.UserId != userId ? PaymentErrors.Forbidden() : payment.ToDto();
    }
}
