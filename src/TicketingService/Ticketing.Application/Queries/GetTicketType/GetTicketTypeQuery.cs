using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Queries.GetTicketType;

public sealed record GetTicketTypeQuery(Guid Id) : IQuery<ResultT<TicketTypeDto>>;
