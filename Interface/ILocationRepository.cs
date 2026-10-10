using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface ILocationRepository
{
    Task<List<MastLocationResponse>> list(MastLocationRequest req);

    Task<ActionResult> create(MastLocationRequest req);
    Task<ActionResult> update(MastLocationRequest req);
    Task<ActionResult> delete(string id);
    Task<MastLocationResponse?> GetByCode(bool check, string code);
}