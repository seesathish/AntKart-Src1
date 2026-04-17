using AK.Products.Application.Interfaces;
using AK.Products.Application.Queries.GetProductsByCategory;
using AK.Products.Domain.Entities;
using AK.Products.Tests.Common;
using FluentAssertions;
using Moq;

namespace AK.Products.Tests.Application.Queries;

public sealed class GetProductsByCategoryQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly GetProductsByCategoryQueryHandler _handler;

    public GetProductsByCategoryQueryHandlerTests()
    {
        _uowMock.Setup(u => u.Products).Returns(_repoMock.Object);
        _handler = new GetProductsByCategoryQueryHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_WithMatchingProducts_ShouldReturnDtoList()
    {
        var products = new List<Product>
        {
            TestDataFactory.CreateMenProduct("SKU-001"),
            TestDataFactory.CreateMenProduct("SKU-002")
        }.AsReadOnly();
        _repoMock.Setup(r => r.GetByCategoryAsync("Shirts", default)).ReturnsAsync(products);

        var result = await _handler.Handle(new GetProductsByCategoryQuery("Shirts"), default);

        result.Should().HaveCount(2);
        result.All(p => p.CategoryName == "Shirts").Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNoMatchingProducts_ShouldReturnEmptyList()
    {
        _repoMock.Setup(r => r.GetByCategoryAsync("NonExistent", default))
            .ReturnsAsync(new List<Product>().AsReadOnly());

        var result = await _handler.Handle(new GetProductsByCategoryQuery("NonExistent"), default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapProductsToDtos()
    {
        var product = TestDataFactory.CreateMenProduct("MEN-SHRT-001");
        _repoMock.Setup(r => r.GetByCategoryAsync("Shirts", default))
            .ReturnsAsync(new List<Product> { product }.AsReadOnly());

        var result = await _handler.Handle(new GetProductsByCategoryQuery("Shirts"), default);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be(product.Name);
        result[0].SKU.Should().Be(product.SKU);
    }
}
