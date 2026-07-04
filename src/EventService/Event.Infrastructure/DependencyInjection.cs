using BuildingBlocks.Infrastructure;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EventsApplication.Contracts;
using Events.Infrastructure.Persistence.Repositories;

namespace Events.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultInterceptors();
        var connectionString = configuration.GetConnectionString("DbConnection");

        services.AddDbContext<EventDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        //Repositories
        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}
