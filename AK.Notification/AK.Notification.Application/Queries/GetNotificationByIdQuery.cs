using AK.Notification.Application.DTOs;
using AK.Notification.Application.Repositories;
using MediatR;

namespace AK.Notification.Application.Queries;

public sealed record GetNotificationByIdQuery(Guid Id, string RequestingUserId)
    : IRequest<NotificationDto?>;

public sealed class GetNotificationByIdQueryHandler(INotificationRepository repository)
    : IRequestHandler<GetNotificationByIdQuery, NotificationDto?>
{
    public async Task<NotificationDto?> Handle(
        GetNotificationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var notification = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (notification is null) return null;

        if (notification.UserId != request.RequestingUserId)
            throw new UnauthorizedAccessException("You do not have permission to view this notification.");

        return notification.ToDto();
    }
}
