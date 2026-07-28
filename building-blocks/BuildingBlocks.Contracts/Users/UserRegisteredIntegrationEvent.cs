namespace BuildingBlocks.Contracts.Users;

public sealed record UserRegisteredIntegrationEvent(
    Guid CorrelationId,
    Guid UserId,
    string Name,
    string Email);
