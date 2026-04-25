using AK.BuildingBlocks.Authentication;
using AK.Notification.Application.Queries;
using MediatR;

namespace AK.Notification.API.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications")
            .RequireAuthorization("authenticated");

        group.MapGet("/", async (
            HttpContext http,
            IMediator mediator,
            int page = 1,
            int pageSize = 20) =>
        {
            var userId = http.GetUserId();
            var result = await mediator.Send(new GetUserNotificationsQuery(userId, page, pageSize));
            return Results.Ok(result);
        })
        .WithName("GetMyNotifications");

        group.MapGet("/{id:guid}", async (Guid id, HttpContext http, IMediator mediator) =>
        {
            var userId = http.GetUserId();
            var isAdmin = http.User.IsInRole("admin");

            if (isAdmin)
            {
                var allResult = await mediator.Send(new GetAllNotificationsQuery(1, int.MaxValue));
                var found = allResult.Items.FirstOrDefault(n => n.Id == id);
                return found is null ? Results.NotFound() : Results.Ok(found);
            }

            try
            {
                var notification = await mediator.Send(new GetNotificationByIdQuery(id, userId));
                return notification is null ? Results.NotFound() : Results.Ok(notification);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .WithName("GetNotificationById");

        app.MapGet("/api/notifications/admin", async (IMediator mediator, int page = 1, int pageSize = 20) =>
        {
            var result = await mediator.Send(new GetAllNotificationsQuery(page, pageSize));
            return Results.Ok(result);
        })
        .WithTags("Notifications")
        .RequireAuthorization("admin")
        .WithName("GetAllNotifications");
    }
}
