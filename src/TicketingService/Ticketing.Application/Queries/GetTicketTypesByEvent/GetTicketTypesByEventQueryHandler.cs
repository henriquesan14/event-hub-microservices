using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Extensions;

namespace Ticketing.Application.Queries.GetTicketTypesByEvent;

public sealed class GetTicketTypesByEventQueryHandler(ITicketingRepository repository)
    : IQueryHandler<GetTicketTypesByEventQuery, ResultT<IReadOnlyList<TicketTypeDto>>>
{
    public async Task<ResultT<IReadOnlyList<TicketTypeDto>>> Handle(
        GetTicketTypesByEventQuery request, CancellationToken ct)
    {
        var entities = await repository.GetTicketTypesByEventAsync(request.EventId, ct);
        return entities.Select(x => x.ToDto()).ToList();
    }
}
