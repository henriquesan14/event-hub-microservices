namespace BuildingBlocks.Contracts.Users;

public sealed record UserPasswordResetRequestedIntegrationEvent(
    Guid CorrelationId,
    Guid UserId,
    string Name,
    string Email,
    string Token,
    DateTime ExpiresAt);
