using AK.Notification.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Infrastructure.Persistence;

public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
    }
}
