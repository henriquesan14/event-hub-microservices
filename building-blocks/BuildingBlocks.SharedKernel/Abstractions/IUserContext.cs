namespace BuildingBlocks.SharedKernel.Abstractions;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Name { get; }
    string? IpAddress { get; }
    string? RefreshToken { get; }
    void SetCookieTokens(string accessToken, string refreshToken);
    void RemoveCookiesToken();
}
