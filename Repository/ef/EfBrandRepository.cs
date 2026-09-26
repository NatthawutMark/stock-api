using Microsoft.EntityFrameworkCore;
using stock_api.Interfaces;
using stock_api.Models;

namespace stock_api.Repositories.EF;

public class EfBrandRepository : EfGenericRepository<MastBrand>, IBrandRepository
{
    public EfBrandRepository(DbContexts context) : base(context) { }

    public async Task<IEnumerable<MastBrand>> GetActiveBrandsAsync()
    {
        return await _dbSet.Where(b => b.IsActive == true).ToListAsync();
    }
}