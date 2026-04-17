using AK.Products.Application.Common;
using AK.Products.Application.DTOs;
using AK.Products.Application.Interfaces;
using MediatR;

namespace AK.Products.Application.Queries.GetProductsByCategory;

public sealed class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IReadOnlyList<ProductDto>>
{
    private readonly IUnitOfWork _uow;

    public GetProductsByCategoryQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken ct)
    {
        var products = await _uow.Products.GetByCategoryAsync(request.Category, ct);
        return ProductMapper.ToDtoList(products);
    }
}
