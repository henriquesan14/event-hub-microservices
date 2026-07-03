using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.SharedKernel.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Secret { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenExpirationInMinutes { get; init; }

    [Range(1, 30)]
    public int RefreshTokenExpirationInDays { get; init; }
}
