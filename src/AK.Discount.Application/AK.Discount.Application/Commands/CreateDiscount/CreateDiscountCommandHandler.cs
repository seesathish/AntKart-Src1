using AK.Discount.Application.Common;
using AK.Discount.Application.DTOs;
using AK.Discount.Application.Interfaces;
using AK.Discount.Domain.Entities;
using AK.Discount.Domain.Enums;
using MediatR;
namespace AK.Discount.Application.Commands.CreateDiscount;
public sealed class CreateDiscountCommandHandler(ICouponRepository repo) : IRequestHandler<CreateDiscountCommand, CouponDto>
{
    public async Task<CouponDto> Handle(CreateDiscountCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        if (await repo.CouponCodeExistsAsync(dto.CouponCode, ct))
            throw new InvalidOperationException($"Coupon code '{dto.CouponCode}' already exists.");
        var coupon = new Coupon
        {
            ProductId = dto.ProductId,
            ProductName = dto.ProductName,
            CouponCode = dto.CouponCode.ToUpperInvariant(),
            Description = dto.Description,
            Amount = dto.Amount,
            DiscountType = Enum.Parse<DiscountType>(dto.DiscountType, true),
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            MinimumQuantity = dto.MinimumQuantity
        };
        var created = await repo.CreateAsync(coupon, ct);
        return created.ToDto();
    }
}
