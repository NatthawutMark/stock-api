using stock_api.Interfaces;
using stock_api.Models;

namespace stock_api.Repositories.EF;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly DbContexts _context;
    public IBrandRepository Brands { get; private set; }
    public IWarehouseRepository Warehouses { get; private set; }
    public ISystemMenuRepository SystemMenus { get; private set; }
    public IAuthRepository Auths { get; private set; }

    public EfUnitOfWork(DbContexts context)
    {
        _context = context;
        Brands = new EfBrandRepository(_context);
        Warehouses = new EfWarehouseRepository(_context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}