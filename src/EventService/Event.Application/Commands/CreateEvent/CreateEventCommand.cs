using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Events.Application.Dtos;
using EventsApplication.Dtos;

namespace EventsApplication.Commands.CreateEvent;

public sealed record CreateEventCommand(string Title, string Description, AddressRequest Address, DateTime StartsAt,
    DateTime EndsAt) : ICommand<ResultT<EventDto>>;


