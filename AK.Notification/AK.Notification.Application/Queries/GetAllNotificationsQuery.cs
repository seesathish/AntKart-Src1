using AK.BuildingBlocks.Common;
using AK.Notification.Application.DTOs;
using AK.Notification.Application.Repositories;
using MediatR;

namespace AK.Notification.Application.Queries;

public sealed record GetAllNotificationsQuery(int Page, int PageSize)
    : IRequest<PagedResult<NotificationDto>>;

public sealed class GetAllNotificationsQueryHandler(INotificationRepository repository)
    : IRequestHandler<GetAllNotificationsQuery, PagedResult<NotificationDto>>
{
    public async Task<PagedResult<NotificationDto>> Handle(
        GetAllNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var total = await repository.CountAllAsync(cancellationToken);
        var dtos = items.Select(n => n.ToDto()).ToList();
        return new PagedResult<NotificationDto>(dtos, total, request.Page, request.PageSize);
    }
}
