using stock_api.Request;

namespace stock_api.Interfaces;

public interface ISystemMenuRepository : IGenericRepository<SystemMenuRequest>
{
    Task<SystemMenuRequest?> GetByCodeAsync(object code);
}