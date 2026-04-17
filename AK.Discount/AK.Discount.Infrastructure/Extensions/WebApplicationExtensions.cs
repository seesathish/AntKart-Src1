using AK.Discount.Infrastructure.Persistence;
using AK.Discount.Infrastructure.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace AK.Discount.Infrastructure.Extensions;
public static class WebApplicationExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DiscountContext>();
        await context.Database.MigrateAsync();
        var seeder = scope.ServiceProvider.GetRequiredService<DiscountSeeder>();
        await seeder.SeedAsync();
    }
}
