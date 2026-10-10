using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IUomRepository
{
    Task<List<MastUomResponse>> list(MastUomRequest req);
    Task<ActionResult> create(MastUomRequest req);
    Task<ActionResult> update(MastUomRequest req);
    Task<ActionResult> delete(string id);
    Task<MastUomResponse?> GetByName(bool check, string name);
}
