using AK.Order.Application.Common.Interfaces;
using AK.Order.Application.Features.CreateOrder;
using AK.Order.Domain.Entities;
using AK.Order.Tests.Common;
using MassTransit;
using OrderEntity = AK.Order.Domain.Entities.Order;
using FluentAssertions;
using Moq;

namespace AK.Order.Tests.Features.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IOrderRepository> _repo = new();
    private readonly Mock<IPublishEndpoint> _publisher = new();

    public CreateOrderCommandHandlerTests()
    {
        _uow.Setup(u => u.Orders).Returns(_repo.Object);
        _repo.Setup(r => r.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderEntity o, CancellationToken _) => o);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    private CreateOrderCommand BuildCommand(
        string userId = "user-123",
        string email = "john@example.com",
        string name = "John Doe")
        => new(userId, email, name, TestDataFactory.CreateOrderDto());

    [Fact]
    public async Task Handle_ValidCommand_ReturnsOrderDto()
    {
        var handler = new CreateOrderCommandHandler(_uow.Object, _publisher.Object);
        var result = await handler.Handle(BuildCommand(), CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be("user-123");
        result.OrderNumber.Should().StartWith("ORD-");
        result.Status.Should().Be("Pending");
        result.PaymentStatus.Should().Be("Pending");
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesOrder()
    {
        var handler = new CreateOrderCommandHandler(_uow.Object, _publisher.Object);
        await handler.Handle(BuildCommand(), CancellationToken.None);

        _repo.Verify(r => r.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_PublishesOrderCreatedEvent()
    {
        var handler = new CreateOrderCommandHandler(_uow.Object, _publisher.Object);
        await handler.Handle(BuildCommand("user-123", "john@example.com", "John Doe"), CancellationToken.None);

        _publisher.Verify(p => p.Publish(
            It.Is<AK.BuildingBlocks.Messaging.IntegrationEvents.OrderCreatedIntegrationEvent>(e =>
                e.UserId == "user-123" &&
                e.CustomerEmail == "john@example.com" &&
                e.CustomerName == "John Doe" &&
                e.OrderNumber.StartsWith("ORD-")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_MapsShippingAddress()
    {
        var handler = new CreateOrderCommandHandler(_uow.Object, _publisher.Object);
        var result = await handler.Handle(BuildCommand(), CancellationToken.None);

        result.ShippingAddress.FullName.Should().Be("John Doe");
        result.ShippingAddress.City.Should().Be("Springfield");
    }

    [Fact]
    public async Task Handle_ValidCommand_StoresCustomerEmail()
    {
        var handler = new CreateOrderCommandHandler(_uow.Object, _publisher.Object);
        var result = await handler.Handle(BuildCommand(email: "customer@test.com"), CancellationToken.None);

        result.Should().NotBeNull();
        _publisher.Verify(p => p.Publish(
            It.Is<AK.BuildingBlocks.Messaging.IntegrationEvents.OrderCreatedIntegrationEvent>(e =>
                e.CustomerEmail == "customer@test.com"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
