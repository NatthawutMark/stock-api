using back_stock.Models;

namespace back_stock.Interfaces;

public interface IBrandRepository : IGenericRepository<MastBrand>
{
    Task<IEnumerable<MastBrand>> GetActiveBrandsAsync();
}