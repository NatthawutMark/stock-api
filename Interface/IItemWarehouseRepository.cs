using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IItemWarehouseRepository
{
    Task<List<MastItemWarehouseResponse>> list(MastItemWarehouseRequest req);
    Task<ActionResult> create(MastItemWarehouseRequest req);
    Task<ActionResult> SyncItemWarehouses(MastItemWarehouseSyncRequest req, string userId);
    Task<List<MastItemWarehouseResponse>> GetByItemId(string itemId);
    Task<MastItemWarehouseResponse?> GetByItemIdAndWarehouseId(string itemId, string warehouseId);
}

