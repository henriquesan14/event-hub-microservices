using AspNetCoreRateLimit;
using BuildingBlocks.SharedKernel.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimitingConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.Configure<IpRateLimitingOptions>(
            configuration.GetSection(IpRateLimitingOptions.SectionName));

        services.Configure<RateLimitOptions>(
            configuration.GetSection("IpRateLimiting"));

        services.Configure<IpRateLimitPolicies>(
            configuration.GetSection("IpRateLimitPolicies"));

        services.AddInMemoryRateLimiting();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }
}
