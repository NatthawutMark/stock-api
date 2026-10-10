using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IDocTypeRepository
{
    Task<List<MastDocTypeResponse>> list(MastDocTypeRequest req);
    Task<ActionResult> create(MastDocTypeRequest req);
    Task<MastDocTypeResponse?> GetByName(bool check, string name, string menuId);
    Task<ActionResult> update(MastDocTypeRequest req);
}

