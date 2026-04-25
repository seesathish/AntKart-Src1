using AK.BuildingBlocks.Common;
using AK.Notification.Application.DTOs;
using AK.Notification.Application.Repositories;
using MediatR;

namespace AK.Notification.Application.Queries;

public sealed record GetUserNotificationsQuery(string UserId, int Page, int PageSize)
    : IRequest<PagedResult<NotificationDto>>;

public sealed class GetUserNotificationsQueryHandler(INotificationRepository repository)
    : IRequestHandler<GetUserNotificationsQuery, PagedResult<NotificationDto>>
{
    public async Task<PagedResult<NotificationDto>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await repository.GetByUserIdAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
        var total = await repository.CountByUserIdAsync(request.UserId, cancellationToken);
        var dtos = items.Select(n => n.ToDto()).ToList();
        return new PagedResult<NotificationDto>(dtos, total, request.Page, request.PageSize);
    }
}
