using AK.Order.Application.Common.Interfaces;
using AK.Order.Infrastructure.Persistence.Repositories;

namespace AK.Order.Infrastructure.Persistence;

internal sealed class UnitOfWork(OrderDbContext db) : IUnitOfWork
{
    private IOrderRepository? _orders;

    public IOrderRepository Orders => _orders ??= new OrderRepository(db);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);

    public void Dispose() => db.Dispose();
}
