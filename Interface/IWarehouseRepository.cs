using stock_api.Models;

namespace stock_api.Interfaces;

public interface IWarehouseRepository : IGenericRepository<MastWarehouse>
{
    Task<IEnumerable<MastWarehouse>> GetActiveWarehousesAsync();
    Task<MastWarehouse?> GetByCodeAsync(object code);
}