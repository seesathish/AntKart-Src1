using AK.Order.Application.Common.DTOs;
using MediatR;

namespace AK.Order.Application.Features.CreateOrder;

public sealed record CreateOrderCommand(string UserId, CreateOrderDto Order) : IRequest<OrderDto>;
