using stock_api.Models;

namespace stock_api.Interfaces;

public interface IBrandRepository : IGenericRepository<MastBrand>
{
    Task<IEnumerable<MastBrand>> GetActiveBrandsAsync();
}