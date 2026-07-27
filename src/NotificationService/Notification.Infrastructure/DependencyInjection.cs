using BuildingBlocks.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Contracts;
using Notification.Infrastructure.Messaging.Consumers;
using Notification.Infrastructure.Persistence;
using Notification.Infrastructure.Persistence.Repositories;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultInterceptors();
        services.AddDbContext<NotificationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DbConnection"));
        });
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<NotificationIntegrationEventConsumer>();
            x.AddEntityFrameworkOutbox<NotificationDbContext>(outbox =>
            {
                outbox.UsePostgres();
                outbox.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(
                    configuration["RabbitMq:Host"] ?? "localhost",
                    configuration["RabbitMq:VirtualHost"] ?? "/",
                    host =>
                    {
                        host.Username(configuration["RabbitMq:Username"] ?? "guest");
                        host.Password(configuration["RabbitMq:Password"] ?? "guest");
                    });

                cfg.ReceiveEndpoint("notification-events", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<NotificationDbContext>(context);
                    endpoint.ConfigureConsumer<NotificationIntegrationEventConsumer>(context);
                });
            });
        });

        return services;
    }
}
