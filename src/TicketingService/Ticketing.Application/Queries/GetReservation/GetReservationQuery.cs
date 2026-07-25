using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Queries.GetReservation;

public sealed record GetReservationQuery(Guid Id) : IQuery<ResultT<ReservationDetailsDto>>;
