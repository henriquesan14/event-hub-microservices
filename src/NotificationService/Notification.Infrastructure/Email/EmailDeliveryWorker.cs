using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.Contracts;

namespace Notification.Infrastructure.Email;

public sealed class EmailDeliveryWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<SmtpOptions> options,
    ILogger<EmailDeliveryWorker> logger) : BackgroundService
{
    private readonly SmtpOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            logger.LogInformation("SMTP email delivery is disabled.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while processing email deliveries.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(Math.Max(_options.PollingIntervalSeconds, 1)),
                stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var templateRenderer =
            scope.ServiceProvider.GetRequiredService<IEmailTemplateRenderer>();
        var deliveries = await repository.GetPendingDeliveriesAsync(
            DateTime.Now,
            Math.Max(_options.BatchSize, 1),
            ct);

        foreach (var delivery in deliveries)
        {
            var now = DateTime.Now;
            try
            {
                var notification = await repository.GetByIdAsync(delivery.NotificationId, ct);
                var recipient = await repository.GetRecipientAsync(delivery.UserId, ct);
                if (notification is null)
                    throw new InvalidOperationException("Notification was not found.");
                if (recipient is null || !recipient.IsActive)
                    throw new InvalidOperationException("Notification recipient is not available.");

                var email = templateRenderer.Render(
                    recipient.Name,
                    recipient.Email,
                    notification);
                await sender.SendAsync(email, ct);
                delivery.MarkSent(now);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                delivery.MarkFailed(
                    exception.GetBaseException().Message,
                    now,
                    Math.Max(_options.MaxAttempts, 1));
                logger.LogWarning(
                    exception,
                    "Email delivery {DeliveryId} failed on attempt {Attempt}.",
                    delivery.Id,
                    delivery.AttemptCount);
            }

            await repository.SaveChangesAsync(ct);
        }
    }
}
