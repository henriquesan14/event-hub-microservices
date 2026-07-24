using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Events.Domain.Enums;
using EventsApplication.Contracts;
using EventsApplication.Errors;

namespace EventsApplication.Commands.DeleteEvent;

public sealed class DeleteEventCommandHandler(IEventRepository eventRepository, IUserContext userContext)
    : ICommandHandler<DeleteEventCommand, Result>
{
    public async Task<Result> Handle(DeleteEventCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return EventErrors.Unauthorized();

        var eventEntity = await eventRepository.GetByIdAsync(request.Id, ct);
        if (eventEntity is null)
            return EventErrors.NotFound(request.Id);
        if (eventEntity.OrganizerId.Value != userId)
            return EventErrors.Forbidden();
        if (eventEntity.Status == EventStatus.Published)
            return Error.Validation("Event.DeletePublished", "A published event must be cancelled before deletion");

        eventRepository.Delete(eventEntity);
        await eventRepository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
