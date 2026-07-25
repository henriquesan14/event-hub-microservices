using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Queries.GetTicketTypesByEvent;

public sealed record GetTicketTypesByEventQuery(Guid EventId) : IQuery<ResultT<IReadOnlyList<TicketTypeDto>>>;
