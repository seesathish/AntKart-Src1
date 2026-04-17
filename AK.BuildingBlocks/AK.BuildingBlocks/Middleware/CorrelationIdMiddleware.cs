using Microsoft.AspNetCore.Http;
namespace AK.BuildingBlocks.Middleware;
public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            correlationId = Guid.NewGuid().ToString();
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId.ToString()))
            await next(context);
    }
}
