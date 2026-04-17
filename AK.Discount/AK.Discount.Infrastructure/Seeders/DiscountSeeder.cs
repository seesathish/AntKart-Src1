using AK.Discount.Domain.Entities;
using AK.Discount.Domain.Enums;
using AK.Discount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace AK.Discount.Infrastructure.Seeders;
public class DiscountSeeder(DiscountContext context, ILogger<DiscountSeeder> logger)
{
    private static readonly (string Gender, string GenderCode, string[] Categories)[] ProductGroups =
    [
        ("Men",   "MEN", ["SHIR","TROU","JEAN","BLAZ","KURT","JACK","SWEA","TSHI","SHOR","SUIT"]),
        ("Women", "WOM", ["SARE","KURT","DRES","LEGG","TOPS","SALW","BLOU","SKIR","PALA","DUNG"]),
        ("Kids",  "KID", ["TSHI","SHOR","FROC","JEAN","SHIR","DRES","LEGG","JACK","SWEA","TRAC"])
    ];

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var existing = await context.Coupons.CountAsync(ct);
        if (existing >= 300) { logger.LogInformation("Discount database already seeded."); return; }

        var rng = new Random(42);
        var coupons = new List<Coupon>();
        var now = DateTime.UtcNow;

        foreach (var (gender, code, categories) in ProductGroups)
        {
            foreach (var cat in categories)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var sku = $"{code}-{cat}-{i:D3}";
                    var discountType = rng.NextDouble() > 0.5 ? DiscountType.Percentage : DiscountType.FlatAmount;
                    var amount = discountType == DiscountType.Percentage
                        ? Math.Round((decimal)(rng.NextDouble() * 30 + 5), 2)   // 5-35%
                        : Math.Round((decimal)(rng.NextDouble() * 200 + 50), 2); // 50-250 flat
                    var validFrom = now.AddDays(-rng.Next(0, 30));
                    var validTo = now.AddDays(rng.Next(30, 180));

                    coupons.Add(new Coupon
                    {
                        ProductId = sku,
                        ProductName = $"{gender} {cat.ToTitleCase()} Product",
                        CouponCode = $"DISC-{sku}-{rng.Next(100, 999)}",
                        Description = $"{(discountType == DiscountType.Percentage ? $"{amount}% off" : $"${amount} off")} on {gender.ToLower()} {cat.ToLower()} item",
                        Amount = amount,
                        DiscountType = discountType,
                        ValidFrom = validFrom,
                        ValidTo = validTo,
                        IsActive = true,
                        MinimumQuantity = rng.Next(1, 4),
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }
        }

        await context.Coupons.AddRangeAsync(coupons, ct);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} discounts into database.", coupons.Count);
    }
}

internal static class StringExtensions
{
    internal static string ToTitleCase(this string s)
        => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..].ToLower();
}
