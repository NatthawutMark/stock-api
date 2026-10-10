using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IBrandRepository
{
    Task<List<MastBrandResponse>> list(MastBrandRequest req);

    Task<ActionResult> create(MastBrandRequest req);
    Task<ActionResult> update(MastBrandRequest req);
    Task<ActionResult> delete(string id);
    Task<MastBrandResponse?> GetByName(bool check, string nameTh);
}