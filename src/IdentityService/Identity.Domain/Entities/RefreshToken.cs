using BuildingBlocks.SharedKernel.Abstractions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public class RefreshToken : Entity<RefreshTokenId>
{
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public UserId UserId { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; } = default!;
    public DateTime? RevokedAt { get; private set; } = default!;
    public string CreatedByIp { get; private set; } = default!;
    public string? ReplacedByToken { get; private set; }
    public string? RevokedByIp { get; private set; }

    public bool IsExpired => DateTime.Now >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;

    public virtual User User { get; private set; } = default!;

    private RefreshToken()
    {

    }

    public RefreshToken(RefreshTokenId id, string token, UserId userId, string createdByIp, DateTime expiresAt)
    {
        Id = id;
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
        RevokedAt = null;
    }

    public void Revoke(string replacedByIp)
    {
        RevokedAt = DateTime.Now;
        RevokedByIp = replacedByIp;
    }

    public void SetReplacedByToken(string replacedByToken)
    {
        ReplacedByToken = replacedByToken;
    }
}
