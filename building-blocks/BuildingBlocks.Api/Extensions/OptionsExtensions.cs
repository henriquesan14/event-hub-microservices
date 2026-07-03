using BuildingBlocks.SharedKernel.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Extensions;

public static class OptionsExtensions
{
    public static IServiceCollection AddDefaultOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Secret),
                "JWT Secret is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer),
                "JWT Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience),
                "JWT Audience is required.")
            .Validate(options => options.AccessTokenExpirationInMinutes > 0,
                "AccessTokenExpirationInMinutes must be greater than zero.")
            .Validate(options => options.RefreshTokenExpirationInDays > 0,
                "RefreshTokenExpirationInDays must be greater than zero.")
            .ValidateOnStart();

        return services;
    }
}
