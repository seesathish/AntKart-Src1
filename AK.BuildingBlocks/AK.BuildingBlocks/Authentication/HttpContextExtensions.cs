using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AK.BuildingBlocks.Authentication;

public static class HttpContextExtensions
{
    // Keycloak JWT: 'sub' is the stable user UUID; 'preferred_username' is the human-readable name.
    // Use 'sub' so the userId is always a unique, stable identifier regardless of username changes.
    public static string GetUserId(this HttpContext ctx) =>
        ctx.User.FindFirst("sub")?.Value
        ?? ctx.User.FindFirst("preferred_username")?.Value
        ?? throw new UnauthorizedAccessException("User identity could not be determined from token.");

    public static string GetUserEmail(this HttpContext ctx) =>
        ctx.User.FindFirst("email")?.Value
        ?? ctx.User.FindFirst(ClaimTypes.Email)?.Value
        ?? string.Empty;

    public static string GetUserDisplayName(this HttpContext ctx) =>
        ctx.User.FindFirst("name")?.Value
        ?? $"{ctx.User.FindFirst("given_name")?.Value} {ctx.User.FindFirst("family_name")?.Value}".Trim()
        ?? ctx.User.FindFirst("preferred_username")?.Value
        ?? "Customer";
}
