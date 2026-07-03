using BuildingBlocks.Infrastructure;
using Identity.Application.Contracts;
using Identity.Domain.Contracts;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultInterceptors();
        var connectionString = configuration.GetConnectionString("DbConnection");

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        //Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        //Services
        services.AddScoped<ITokenService, TokenService>();

        services.AddSingleton<IPasswordCheck, PasswordService>();
        services.AddSingleton<IPasswordHash, PasswordService>();
        return services;
    }
}
