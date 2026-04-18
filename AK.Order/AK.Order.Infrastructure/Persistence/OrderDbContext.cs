using AK.Order.Domain.Entities;
using OrderEntity = AK.Order.Domain.Entities.Order;
using AK.Order.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AK.Order.Infrastructure.Persistence;

public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
    }
}
