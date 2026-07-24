using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Dtos;

namespace EventsApplication.Queries.GetEventById;

public sealed record GetEventByIdQuery(Guid Id) : IQuery<ResultT<EventDto>>;
