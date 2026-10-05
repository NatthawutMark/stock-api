using Microsoft.AspNetCore.Mvc;
using stock_api.Models;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IBrandRepository
{
    Task<List<MastBrandResponse>> list(MastBrandRequest req);

    Task<ActionResult> create(MastBrandRequest req);
    Task<MastBrandResponse?> GetByName(bool check, string nameTh);
}