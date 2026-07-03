namespace Identity.Application.Dtos;

public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);