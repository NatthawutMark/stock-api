using Microsoft.EntityFrameworkCore;
using back_stock.Interfaces;
using back_stock.Models;

namespace back_stock.Repositories.EF;

public class EfBrandRepository : EfGenericRepository<MastBrand>, IBrandRepository
{
    public EfBrandRepository(DbContexts context) : base(context) { }

    public async Task<IEnumerable<MastBrand>> GetActiveBrandsAsync()
    {
        return await _dbSet.Where(b => b.IsActive == true).ToListAsync();
    }
}