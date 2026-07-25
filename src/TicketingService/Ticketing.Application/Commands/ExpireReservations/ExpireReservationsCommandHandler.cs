using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Contracts;

namespace Ticketing.Application.Commands.ExpireReservations;

public sealed class ExpireReservationsCommandHandler(ITicketingRepository repository)
    : ICommandHandler<ExpireReservationsCommand, ResultT<int>>
{
    public async Task<ResultT<int>> Handle(ExpireReservationsCommand request, CancellationToken ct)
    {
        var reservations = await repository.GetExpiredReservationsAsync(DateTime.Now, ct);
        foreach (var reservation in reservations)
        {
            reservation.Release(true);
            reservation.TicketType.Release(reservation.Quantity);
        }

        if (reservations.Count > 0) await repository.SaveChangesAsync(ct);
        return reservations.Count;
    }
}
