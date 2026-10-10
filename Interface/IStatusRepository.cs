using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IStatusRepository
{
    Task<List<MastStatusResponse>> list(MastStatusRequest req);
    Task<ActionResult> create(MastStatusRequest req);
    Task<MastStatusResponse?> GetByCode(bool check, string code);
}

