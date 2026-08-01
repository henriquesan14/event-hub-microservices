using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Ticketing.Infrastructure.Messaging.Consumers;
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

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<OrderExpiredConsumer>();
            x.AddConsumer<PaymentApprovedConsumer>();
            x.AddConsumer<PaymentFailedConsumer>();
            x.AddConsumer<PaymentRefundedConsumer>();
            x.AddEntityFrameworkOutbox<TicketingDbContext>(outbox =>
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

                cfg.ReceiveEndpoint("ticketing-order-cancelled", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<TicketingDbContext>(context);
                    endpoint.ConfigureConsumer<OrderCancelledConsumer>(context);
                });

                cfg.ReceiveEndpoint("ticketing-payment-approved", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<TicketingDbContext>(context);
                    endpoint.ConfigureConsumer<PaymentApprovedConsumer>(context);
                });

                cfg.ReceiveEndpoint("ticketing-payment-failed", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<TicketingDbContext>(context);
                    endpoint.ConfigureConsumer<PaymentFailedConsumer>(context);
                });

                cfg.ReceiveEndpoint("ticketing-payment-refunded", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<TicketingDbContext>(context);
                    endpoint.ConfigureConsumer<PaymentRefundedConsumer>(context);
                });

                cfg.ReceiveEndpoint("ticketing-order-expired", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<TicketingDbContext>(context);
                    endpoint.ConfigureConsumer<OrderExpiredConsumer>(context);
                });
            });
        });
        return services;
    }
}
