using BuildingBlocks.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Contracts;
using Payment.Infrastructure.Messaging.Consumers;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Persistence.Repositories;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultInterceptors();
        services.AddDbContext<PaymentDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DbConnection"));
        });

        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<OrderExpiredConsumer>();
            x.AddEntityFrameworkOutbox<PaymentDbContext>(outbox =>
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

                cfg.ReceiveEndpoint("payment-order-created", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<PaymentDbContext>(context);
                    endpoint.ConfigureConsumer<OrderCreatedConsumer>(context);
                });

                cfg.ReceiveEndpoint("payment-order-cancelled", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<PaymentDbContext>(context);
                    endpoint.ConfigureConsumer<OrderCancelledConsumer>(context);
                });

                cfg.ReceiveEndpoint("payment-order-expired", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<PaymentDbContext>(context);
                    endpoint.ConfigureConsumer<OrderExpiredConsumer>(context);
                });
            });
        });

        return services;
    }
}
