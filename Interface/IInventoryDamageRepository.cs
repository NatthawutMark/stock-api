using Microsoft.AspNetCore.Mvc;
using stock_api.Request;
using static stock_api.Request.InventoryRequest;
using static stock_api.Response.InventoryResponse;

namespace stock_api.Interfaces;

public interface IInventoryDamageRepository
{
    Task<List<InventoryDmgList>> ListAll(InventoryDmgRequest req);
    Task<List<InventoryDmgList>> List(InventoryDmgRequest req);
    Task<int> GetTotal(InventoryDmgRequest req);
}
