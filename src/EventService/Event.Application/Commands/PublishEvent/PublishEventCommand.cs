using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Dtos;

namespace EventsApplication.Commands.PublishEvent;

public sealed record PublishEventCommand(Guid Id) : ICommand<ResultT<EventDto>>;
