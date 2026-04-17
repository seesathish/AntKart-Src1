using AK.Products.Application.DTOs;
using AK.Products.Domain.Entities;

namespace AK.Products.Application.Common;

public static class ProductMapper
{
    public static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Description, p.SKU, p.Brand,
        p.Gender.ToString(), p.Status.ToString(),
        p.CategoryName, p.SubCategoryName,
        p.Price, p.Currency, p.DiscountPrice,
        p.StockQuantity, p.Sizes, p.Colors, p.ImageUrls,
        p.Material, p.IsFeatured, p.Rating, p.ReviewCount,
        p.Tags, p.CreatedAt, p.UpdatedAt
    );

    public static IReadOnlyList<ProductDto> ToDtoList(IEnumerable<Product> products) =>
        products.Select(ToDto).ToList().AsReadOnly();
}
