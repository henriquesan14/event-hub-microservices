using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Events.Domain.ValueObjects;
using EventsApplication.Contracts;
using EventsApplication.Dtos;
using EventsApplication.Errors;
using EventsApplication.Extensions;

namespace EventsApplication.Commands.UpdateEvent;

public sealed class UpdateEventCommandHandler(IEventRepository eventRepository, IUserContext userContext)
    : ICommandHandler<UpdateEventCommand, ResultT<EventDto>>
{
    public async Task<ResultT<EventDto>> Handle(UpdateEventCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return EventErrors.Unauthorized();

        var eventEntity = await eventRepository.GetByIdAsync(request.Id, ct);
        if (eventEntity is null)
            return EventErrors.NotFound(request.Id);
        if (eventEntity.OrganizerId.Value != userId)
            return EventErrors.Forbidden();

        var address = new Address(
            request.Address.Street, request.Address.Number, request.Address.District,
            request.Address.City, request.Address.State, request.Address.Country, request.Address.ZipCode);

        eventEntity.Update(request.Title, request.Description, address, request.StartsAt, request.EndsAt);
        await eventRepository.SaveChangesAsync(ct);
        return eventEntity.ToDto();
    }
}
