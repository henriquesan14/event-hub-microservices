using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;
using Ticketing.Application.Dtos;
using Ticketing.Application.Errors;
using Ticketing.Application.Extensions;

namespace Ticketing.Application.Queries.GetTicketType;

public sealed class GetTicketTypeQueryHandler(ITicketingRepository repository)
    : IQueryHandler<GetTicketTypeQuery, ResultT<TicketTypeDto>>
{
    public async Task<ResultT<TicketTypeDto>> Handle(GetTicketTypeQuery request, CancellationToken ct)
    {
        var entity = await repository.GetTicketTypeAsync(request.Id, ct);
        return entity is null ? TicketingErrors.TicketTypeNotFound(request.Id) : entity.ToDto();
    }
}
