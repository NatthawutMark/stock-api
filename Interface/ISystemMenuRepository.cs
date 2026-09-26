using stock_api.Models;

namespace stock_api.Interfaces;

public interface ISystemMenuRepository : IGenericRepository<SysMenu>
{
    // Task<IEnumerable<SysMenu>> GetActiveMenusAsync();
    Task<SysMenu?> GetByCodeAsync(object code);
}