using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IWarehouseRepository
{
    Task<IEnumerable<MastWarehouseResponse>> GetAllAsync();
    Task<MastWarehouseResponse?> GetByIdAsync(object id);
    Task<MastWarehouseResponse?> GetByCodeAsync(object code);
    Task<IEnumerable<MastWarehouseResponse>> GetActiveWarehousesAsync();
    Task AddAsync(MastWarehouseRequest entity);
    void Update(MastWarehouseRequest entity);
    void Remove(MastWarehouseRequest entity);
}