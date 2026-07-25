using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Extensions;
using Ticketing.Domain.Entities;

namespace Ticketing.Application.Commands.CreateTicketType;

public sealed class CreateTicketTypeCommandHandler(
    ITicketingRepository repository,
    IUserContext userContext)
    : ICommandHandler<CreateTicketTypeCommand, ResultT<TicketTypeDto>>
{
    public async Task<ResultT<TicketTypeDto>> Handle(CreateTicketTypeCommand request, CancellationToken ct)
    {
        if (userContext.UserId is null)
            return Ticketing.Application.Errors.TicketingErrors.Unauthorized();

        var entity = TicketType.Create(
            request.EventId, request.Name, request.Description, request.Price, request.Currency,
            request.TotalQuantity, request.SalesStart, request.SalesEnd);
        await repository.AddTicketTypeAsync(entity, ct);
        await repository.SaveChangesAsync(ct);
        return entity.ToDto();
    }
}
