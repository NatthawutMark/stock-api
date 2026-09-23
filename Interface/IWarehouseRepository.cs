using back_stock.Models;

namespace back_stock.Interfaces;

public interface IWarehouseRepository : IGenericRepository<MastWarehouse>
{
    Task<IEnumerable<MastWarehouse>> GetActiveWarehousesAsync();
}