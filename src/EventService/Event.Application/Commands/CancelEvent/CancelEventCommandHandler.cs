using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Contracts;
using EventsApplication.Dtos;
using EventsApplication.Errors;
using EventsApplication.Extensions;

namespace EventsApplication.Commands.CancelEvent;

public sealed class CancelEventCommandHandler(IEventRepository eventRepository, IUserContext userContext)
    : ICommandHandler<CancelEventCommand, ResultT<EventDto>>
{
    public async Task<ResultT<EventDto>> Handle(CancelEventCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return EventErrors.Unauthorized();

        var eventEntity = await eventRepository.GetByIdAsync(request.Id, ct);
        if (eventEntity is null)
            return EventErrors.NotFound(request.Id);
        if (eventEntity.OrganizerId.Value != userId)
            return EventErrors.Forbidden();

        eventEntity.Cancel();
        await eventRepository.SaveChangesAsync(ct);
        return eventEntity.ToDto();
    }
}
