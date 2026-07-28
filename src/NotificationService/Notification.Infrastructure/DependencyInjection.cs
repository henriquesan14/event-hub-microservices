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
using Notification.Infrastructure.Email;

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
        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .Validate(
                options => !options.Enabled ||
                           (!string.IsNullOrWhiteSpace(options.Host) &&
                            !string.IsNullOrWhiteSpace(options.Username) &&
                            !string.IsNullOrWhiteSpace(options.Password) &&
                            !string.IsNullOrWhiteSpace(options.FromAddress)),
                "SMTP Host, Username, Password and FromAddress are required when email delivery is enabled.")
            .ValidateOnStart();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddOptions<EmailLinksOptions>()
            .Bind(configuration.GetSection(EmailLinksOptions.SectionName));
        services.AddSingleton<IEmailTemplateRenderer, HtmlEmailTemplateRenderer>();
        services.AddHostedService<EmailDeliveryWorker>();

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
