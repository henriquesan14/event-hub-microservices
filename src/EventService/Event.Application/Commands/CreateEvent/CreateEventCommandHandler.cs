using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using EventsApplication.Contracts;
using EventsApplication.Dtos;
using EventsApplication.Extensions;

namespace EventsApplication.Commands.CreateEvent;

public sealed class CreateEventCommandHandler(IEventRepository eventRepository, IUserContext userContext) : ICommandHandler<CreateEventCommand, ResultT<EventDto>>
{
    public async Task<ResultT<EventDto>> Handle(CreateEventCommand request, CancellationToken ct)
    {
        var eventEntity = Event.Create(EventId.New(), request.Title, request.Description, request.Address, 
            request.StartsAt, request.EndsAt, UserId.Of(userContext.UserId!.Value));
        await eventRepository.AddAsync(eventEntity,  ct);

        await eventRepository.SaveChangesAsync(ct);

        return eventEntity.ToDto();
    }
}
