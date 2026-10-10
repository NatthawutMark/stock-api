using Microsoft.AspNetCore.Mvc;
using static stock_api.response.MasterResponse;
using static stock_api.Request.MasterRequest;

namespace stock_api.Interfaces;

public interface IItemRepository
{
    Task<List<MastItemResponse>> GetAll(MastItemRequest req);
    Task<ActionResult> create(MastItemRequest req);
    Task<ActionResult> update(MastItemRequest req);
    Task<ActionResult> delete(string id);
    Task<MastItemResponse?> GetByCode(string itemCode);
}
