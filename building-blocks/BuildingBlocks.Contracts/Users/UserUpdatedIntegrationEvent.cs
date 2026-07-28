namespace BuildingBlocks.Contracts.Users;

public sealed record UserUpdatedIntegrationEvent(
    Guid CorrelationId,
    Guid UserId,
    string Name,
    string Email);
