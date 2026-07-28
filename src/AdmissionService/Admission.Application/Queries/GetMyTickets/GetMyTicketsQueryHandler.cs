using Admission.Application.Contracts;
using Admission.Application.Dtos;
using Admission.Application.Errors;
using Admission.Application.Extensions;
using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Admission.Application.Queries.GetMyTickets;

public sealed class GetMyTicketsQueryHandler(
    IAdmissionRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetMyTicketsQuery, ResultT<IReadOnlyList<AdmissionTicketDto>>>
{
    public async Task<ResultT<IReadOnlyList<AdmissionTicketDto>>> Handle(
        GetMyTicketsQuery request,
        CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return AdmissionErrors.Unauthorized();
        var tickets = await repository.GetByUserAsync(userId, ct);
        return tickets.Select(x => x.ToDto()).ToList();
    }
}
