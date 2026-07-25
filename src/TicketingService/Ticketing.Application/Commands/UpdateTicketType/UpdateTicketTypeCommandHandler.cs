using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Errors;
using Ticketing.Application.Extensions;

namespace Ticketing.Application.Commands.UpdateTicketType;

public sealed class UpdateTicketTypeCommandHandler(
    ITicketingRepository repository,
    IUserContext userContext)
    : ICommandHandler<UpdateTicketTypeCommand, ResultT<TicketTypeDto>>
{
    public async Task<ResultT<TicketTypeDto>> Handle(UpdateTicketTypeCommand request, CancellationToken ct)
    {
        var entity = await repository.GetTicketTypeAsync(request.Id, ct);
        if (entity is null) return TicketingErrors.TicketTypeNotFound(request.Id);
        if (userContext.UserId is not Guid userId) return TicketingErrors.Unauthorized();
        if (entity.CreatedBy != userId) return TicketingErrors.TicketTypeForbidden();

        entity.Update(
            request.Name, request.Description, request.Price, request.Currency,
            request.TotalQuantity, request.SalesStart, request.SalesEnd, request.Active);
        await repository.SaveChangesAsync(ct);
        return entity.ToDto();
    }
}
