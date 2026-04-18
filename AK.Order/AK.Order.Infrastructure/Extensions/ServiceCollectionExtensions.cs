using AK.Order.Application.Common.Interfaces;
using AK.Order.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AK.Order.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' is missing.");

        services.AddDbContext<OrderDbContext>(opts =>
            opts.UseNpgsql(connStr));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
