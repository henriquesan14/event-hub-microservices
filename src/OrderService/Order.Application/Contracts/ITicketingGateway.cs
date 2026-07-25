namespace Order.Application.Contracts;

public interface ITicketingGateway
{
    Task<ReservationSnapshot?> GetReservationAsync(Guid id, CancellationToken ct);
    Task<bool> ReleaseReservationAsync(Guid id, CancellationToken ct);
}

public sealed record ReservationSnapshot(
    Guid Id,
    Guid UserId,
    Guid TicketTypeId,
    Guid EventId,
    string TicketName,
    decimal UnitPrice,
    string Currency,
    int Quantity,
    DateTime ExpiresAt,
    string Status);
