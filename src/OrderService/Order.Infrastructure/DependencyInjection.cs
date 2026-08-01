using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Contracts;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Persistence.Repositories;
using MassTransit;
using Order.Infrastructure.Messaging.Consumers;
using Order.Infrastructure.Messaging.Sagas;

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

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReservationCreatedConsumer>();
            x.AddConsumer<PaymentApprovedConsumer>();
            x.AddConsumer<PaymentFailedConsumer>();
            x.AddSagaStateMachine<PurchaseStateMachine, PurchaseState>()
                .EntityFrameworkRepository(repository =>
                {
                    repository.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    repository.ExistingDbContext<OrderDbContext>();
                    repository.UsePostgres();
                });
            x.AddEntityFrameworkOutbox<OrderDbContext>(outbox =>
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

                cfg.ReceiveEndpoint("order-reservation-created", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<OrderDbContext>(context);
                    endpoint.ConfigureConsumer<ReservationCreatedConsumer>(context);
                });

                cfg.ReceiveEndpoint("order-payment-approved", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<OrderDbContext>(context);
                    endpoint.ConfigureConsumer<PaymentApprovedConsumer>(context);
                });

                cfg.ReceiveEndpoint("order-payment-failed", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<OrderDbContext>(context);
                    endpoint.ConfigureConsumer<PaymentFailedConsumer>(context);
                });

                cfg.ReceiveEndpoint("order-purchase-saga", endpoint =>
                {
                    endpoint.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<OrderDbContext>(context);
                    endpoint.ConfigureSaga<PurchaseState>(context);
                });
            });
        });
        return services;
    }
}
