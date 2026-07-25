using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Ticketing.Application.Commands.ExpireReservations;

public sealed record ExpireReservationsCommand : ICommand<ResultT<int>>;
