using Microsoft.EntityFrameworkCore;
using stock_api.Interfaces;
using stock_api.Models;

namespace stock_api.Repositories.EF;

public class EfWarehouseRepository : EfGenericRepository<MastWarehouse>, IWarehouseRepository
{
    public EfWarehouseRepository(DbContexts context) : base(context) { }

    public async Task<IEnumerable<MastWarehouse>> GetActiveWarehousesAsync()
    {
        return await _dbSet.Where(b => b.IsActive == true).ToListAsync();
    }


    public async Task<MastWarehouse?> GetByCodeAsync(object code)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Code == code);
    }
}