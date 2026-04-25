using AK.Notification.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AK.Notification.Infrastructure.Services;

internal class NotificationCleanupService(
    IServiceScopeFactory scopeFactory,
    IOptions<NotificationSettings> options,
    ILogger<NotificationCleanupService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = CalculateDelayUntilNextRun();
            logger.LogInformation("Notification cleanup scheduled in {Delay}", delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            await RunCleanupAsync(stoppingToken);
        }
    }

    private async Task RunCleanupAsync(CancellationToken ct)
        => await RunCleanupInternalAsync(ct);

    internal async Task RunCleanupInternalAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

            var cutoff = DateTimeOffset.UtcNow.AddDays(-options.Value.RetentionDays);
            var deleted = await repository.DeleteOlderThanAsync(cutoff, ct);

            logger.LogInformation("Notification cleanup: deleted {Count} notifications older than {Cutoff}", deleted, cutoff);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during notification cleanup");
        }
    }

    private static TimeSpan CalculateDelayUntilNextRun()
    {
        var now = DateTimeOffset.UtcNow;
        var nextRun = new DateTimeOffset(now.Year, now.Month, now.Day, 2, 0, 0, TimeSpan.Zero);

        if (nextRun <= now)
            nextRun = nextRun.AddDays(1);

        return nextRun - now;
    }
}
