using Admission.Application.Contracts;
using Admission.Application.Dtos;
using Admission.Application.Errors;
using Admission.Application.Extensions;
using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Admission.Application.Commands.CheckInTicket;

public sealed class CheckInTicketCommandHandler(
    IAdmissionRepository repository,
    IUserContext userContext)
    : ICommandHandler<CheckInTicketCommand, ResultT<AdmissionTicketDto>>
{
    public async Task<ResultT<AdmissionTicketDto>> Handle(
        CheckInTicketCommand request,
        CancellationToken ct)
    {
        if (userContext.UserId is not Guid operatorId)
            return AdmissionErrors.Unauthorized();
        var ticket = await repository.GetByCodeAsync(request.Code, ct);
        if (ticket is null) return AdmissionErrors.NotFound();

        try
        {
            ticket.CheckIn(operatorId, userContext.IpAddress ?? "unknown", DateTime.Now);
        }
        catch (DomainException exception)
        {
            return AdmissionErrors.Invalid(exception.Message);
        }

        await repository.SaveChangesAsync(ct);
        return ticket.ToDto();
    }
}
