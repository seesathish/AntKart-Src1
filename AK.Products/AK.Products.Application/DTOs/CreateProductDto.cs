using AK.Products.Domain.Enums;

namespace AK.Products.Application.DTOs;

public sealed record CreateProductDto(
    string Name,
    string Description,
    string SKU,
    string Brand,
    Gender Gender,
    string CategoryName,
    string? SubCategoryName,
    decimal Price,
    string Currency,
    int StockQuantity,
    List<string> Sizes,
    List<string> Colors,
    string? Material
);
