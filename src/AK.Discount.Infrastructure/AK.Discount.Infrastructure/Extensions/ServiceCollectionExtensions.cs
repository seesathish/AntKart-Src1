using AK.Discount.Application.Interfaces;
using AK.Discount.Infrastructure.Persistence;
using AK.Discount.Infrastructure.Persistence.Repositories;
using AK.Discount.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace AK.Discount.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscountInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DiscountDb") ?? "Data Source=discount.db";
        services.AddDbContext<DiscountContext>(opts => opts.UseSqlite(connectionString));
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<DiscountSeeder>();
        return services;
    }
}
