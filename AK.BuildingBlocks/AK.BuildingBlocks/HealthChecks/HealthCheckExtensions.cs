using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace AK.BuildingBlocks.HealthChecks;
public static class HealthCheckExtensions
{
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
        return services;
    }
    public static WebApplication MapDefaultHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions { AllowCachingResponses = false });
        return app;
    }
}
