namespace BuildingBlocks.SharedKernel.Abstractions;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Name { get; }
    bool IsInRole(string role);
    string? IpAddress { get; }
    string? RefreshToken { get; }
    void SetCookieTokens(string accessToken, string refreshToken);
    void RemoveCookiesToken();
}
