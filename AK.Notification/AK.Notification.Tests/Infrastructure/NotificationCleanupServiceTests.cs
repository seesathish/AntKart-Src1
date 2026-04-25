using AK.Notification.Application.Repositories;
using AK.Notification.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace AK.Notification.Tests.Infrastructure;

public class NotificationCleanupServiceTests
{
    [Fact]
    public async Task ExecuteAsync_DeletesOldNotifications()
    {
        var repoMock = new Mock<INotificationRepository>();
        repoMock.Setup(r => r.DeleteOlderThanAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var services = new ServiceCollection();
        services.AddScoped<INotificationRepository>(_ => repoMock.Object);
        var serviceProvider = services.BuildServiceProvider();

        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var settings = Options.Create(new NotificationSettings { RetentionDays = 30 });

        var service = new TestableNotificationCleanupService(scopeFactory, settings,
            NullLogger<NotificationCleanupService>.Instance);

        await service.RunCleanupPublicAsync(CancellationToken.None);

        repoMock.Verify(r => r.DeleteOlderThanAsync(
            It.Is<DateTimeOffset>(d => d < DateTimeOffset.UtcNow.AddDays(-29)),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

internal sealed class TestableNotificationCleanupService(
    IServiceScopeFactory scopeFactory,
    IOptions<NotificationSettings> options,
    Microsoft.Extensions.Logging.ILogger<NotificationCleanupService> logger)
    : NotificationCleanupService(scopeFactory, options, logger)
{
    public Task RunCleanupPublicAsync(CancellationToken ct)
        => RunCleanupInternalAsync(ct);
}
