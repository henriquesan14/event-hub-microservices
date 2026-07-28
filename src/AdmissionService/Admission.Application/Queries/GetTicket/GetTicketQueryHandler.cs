using Admission.Application.Contracts;
using Admission.Application.Dtos;
using Admission.Application.Errors;
using Admission.Application.Extensions;
using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Admission.Application.Queries.GetTicket;

public sealed class GetTicketQueryHandler(
    IAdmissionRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetTicketQuery, ResultT<AdmissionTicketDto>>
{
    public async Task<ResultT<AdmissionTicketDto>> Handle(
        GetTicketQuery request,
        CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return AdmissionErrors.Unauthorized();
        var ticket = await repository.GetByIdAsync(request.Id, ct);
        if (ticket is null) return AdmissionErrors.NotFound();
        if (ticket.UserId != userId) return AdmissionErrors.Forbidden();
        return ticket.ToDto();
    }
}
