using AK.Products.Application.DTOs;
using AK.Products.Domain.Enums;
using MediatR;

namespace AK.Products.Application.Queries.GetProducts;

public sealed record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    Gender? Gender = null,
    string? Category = null,
    string? SearchTerm = null,
    bool? IsFeatured = null
) : IRequest<PagedResult<ProductDto>>;
