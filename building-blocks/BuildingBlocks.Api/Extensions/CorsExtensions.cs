using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IHostEnvironment env)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                }
                else
                {
                    builder.WithOrigins("")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                }
            });
        });

        return services;
    }
}
