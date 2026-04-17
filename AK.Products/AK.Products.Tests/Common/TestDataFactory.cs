using AK.Products.Application.DTOs;
using AK.Products.Domain.Entities;
using AK.Products.Domain.Enums;

namespace AK.Products.Tests.Common;

public static class TestDataFactory
{
    public static Product CreateMenProduct(string? sku = null) =>
        Product.Create("Men's Classic Shirt", "A premium men's shirt", sku ?? "MEN-SHRT-001",
            "ArrowMen", Gender.Men, "Shirts", null, 999.99m, "USD", 50,
            ["S", "M", "L", "XL"], ["White", "Blue"], "Cotton");

    public static Product CreateWomenProduct(string? sku = null) =>
        Product.Create("Women's Floral Dress", "A beautiful floral dress", sku ?? "WOM-DRES-001",
            "Biba", Gender.Women, "Dresses", null, 1499.99m, "USD", 30,
            ["XS", "S", "M"], ["Red", "Pink"], "Chiffon");

    public static Product CreateKidsProduct(string? sku = null) =>
        Product.Create("Kids Cartoon Tee", "Fun cartoon t-shirt for kids", sku ?? "KID-TSHI-001",
            "H&M Kids", Gender.Kids, "T-Shirts", null, 399.99m, "USD", 100,
            ["3-4Y", "4-5Y", "5-6Y"], ["Blue", "Yellow"], "Soft Cotton");

    public static CreateProductDto CreateProductDto(string? sku = null) => new(
        "Test Shirt", "A test shirt description", sku ?? "TEST-001",
        "TestBrand", Gender.Men, "Shirts", null, 599.99m, "USD", 25,
        ["S", "M", "L"], ["White"], "Cotton");

    public static UpdateProductDto UpdateProductDto() => new(
        "Updated Shirt", "Updated description", "UpdatedBrand", 699.99m, 30, "Linen");
}
