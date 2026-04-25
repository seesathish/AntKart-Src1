using AK.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AK.Notification.API.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationsDbContext>>();

        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations.");
            throw;
        }
    }
}
