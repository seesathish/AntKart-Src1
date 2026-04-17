using AK.Products.Domain.Entities;
using AK.Products.Application.Interfaces;
using AK.Products.Application.Queries.GetProductById;
using AK.Products.Tests.Common;
using FluentAssertions;
using Moq;

namespace AK.Products.Tests.Application.Queries;

public sealed class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _uowMock.Setup(u => u.Products).Returns(_repoMock.Object);
        _handler = new GetProductByIdQueryHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingProduct_ShouldReturnProductDto()
    {
        var product = TestDataFactory.CreateMenProduct();
        _repoMock.Setup(r => r.GetByIdAsync(product.Id, default)).ReturnsAsync(product);

        var result = await _handler.Handle(new GetProductByIdQuery(product.Id), default);

        result.Should().NotBeNull();
        result!.Name.Should().Be(product.Name);
        result.SKU.Should().Be(product.SKU);
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ShouldReturnNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync("nonexistent", default))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(new GetProductByIdQuery("nonexistent"), default);

        result.Should().BeNull();
    }
}
