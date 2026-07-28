namespace BuildingBlocks.Contracts.Tickets;

public sealed record AdmissionTicketsIssuedIntegrationEvent(
    Guid CorrelationId,
    Guid PaymentId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    Guid EventId,
    int Quantity,
    DateTime IssuedAt,
    IReadOnlyList<IssuedAdmissionTicket> Tickets);

public sealed record IssuedAdmissionTicket(
    Guid TicketId,
    string TicketName,
    string Code);
