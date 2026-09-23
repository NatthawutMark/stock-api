using Microsoft.EntityFrameworkCore;
using back_stock.Interfaces;
using back_stock.Models;

namespace back_stock.Repositories.EF;

public class EfWarehouseRepository : EfGenericRepository<MastWarehouse>, IWarehouseRepository
{
    public EfWarehouseRepository(DbContexts context) : base(context) { }

    public async Task<IEnumerable<MastWarehouse>> GetActiveWarehousesAsync()
    {
        return await _dbSet.Where(b => b.IsActive == true).ToListAsync();
    }
}