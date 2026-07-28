using Admission.Application.Contracts;
using Admission.Infrastructure.Messaging.Consumers;
using Admission.Infrastructure.Persistence;
using Admission.Infrastructure.Persistence.Repositories;
using BuildingBlocks.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultInterceptors();
        services.AddDbContext<AdmissionDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DbConnection"));
        });
        services.AddScoped<IAdmissionRepository, AdmissionRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReservationConfirmedConsumer>();
            x.AddEntityFrameworkOutbox<AdmissionDbContext>(outbox =>
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

                cfg.ReceiveEndpoint("admission-reservation-confirmed", endpoint =>
                {
                    endpoint.UseMessageRetry(retry =>
                        retry.Interval(3, TimeSpan.FromSeconds(2)));
                    endpoint.UseEntityFrameworkOutbox<AdmissionDbContext>(context);
                    endpoint.ConfigureConsumer<ReservationConfirmedConsumer>(context);
                });
            });
        });

        return services;
    }
}
