using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Contracts;
using EventsApplication.Dtos;
using EventsApplication.Errors;
using EventsApplication.Extensions;

namespace EventsApplication.Commands.PublishEvent;

public sealed class PublishEventCommandHandler(IEventRepository eventRepository, IUserContext userContext)
    : ICommandHandler<PublishEventCommand, ResultT<EventDto>>
{
    public async Task<ResultT<EventDto>> Handle(PublishEventCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return EventErrors.Unauthorized();

        var eventEntity = await eventRepository.GetByIdAsync(request.Id, ct);
        if (eventEntity is null)
            return EventErrors.NotFound(request.Id);
        if (!userContext.IsInRole("Admin") && eventEntity.OrganizerId.Value != userId)
            return EventErrors.Forbidden();

        eventEntity.Publish();
        await eventRepository.SaveChangesAsync(ct);
        return eventEntity.ToDto();
    }
}
