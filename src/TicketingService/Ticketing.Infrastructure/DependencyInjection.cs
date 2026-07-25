using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Application.Contracts;
using Ticketing.Infrastructure.Persistence;
using Ticketing.Infrastructure.Persistence.Repositories;

namespace Ticketing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultInterceptors();
        services.AddDbContext<TicketingDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DbConnection"));
        });
        services.AddScoped<ITicketingRepository, TicketingRepository>();
        return services;
    }
}
