using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Contracts;
using Order.Infrastructure.Integrations;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Persistence.Repositories;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultInterceptors();
        services.AddDbContext<OrderDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DbConnection"));
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddTransient<ForwardAccessTokenHandler>();
        services.AddHttpClient<ITicketingGateway, TicketingGateway>(client =>
        {
            var baseUrl = configuration["Services:TicketingUrl"]
                ?? throw new InvalidOperationException("Services:TicketingUrl is required.");
            client.BaseAddress = new Uri(baseUrl);
        }).AddHttpMessageHandler<ForwardAccessTokenHandler>();
        return services;
    }
}
