using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Errors;

namespace Ticketing.Application.Commands.DeleteTicketType;

public sealed class DeleteTicketTypeCommandHandler(
    ITicketingRepository repository,
    IUserContext userContext)
    : ICommandHandler<DeleteTicketTypeCommand, Result>
{
    public async Task<Result> Handle(DeleteTicketTypeCommand request, CancellationToken ct)
    {
        var entity = await repository.GetTicketTypeAsync(request.Id, ct);
        if (entity is null) return TicketingErrors.TicketTypeNotFound(request.Id);
        if (userContext.UserId is not Guid userId) return TicketingErrors.Unauthorized();
        if (!userContext.IsInRole("Admin") && entity.CreatedBy != userId)
            return TicketingErrors.TicketTypeForbidden();
        if (entity.AvailableQuantity != entity.TotalQuantity)
            return Error.Validation("Ticketing.TicketInUse", "Ticket types with reservations or sales cannot be deleted");

        repository.DeleteTicketType(entity);
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
