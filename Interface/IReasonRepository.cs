using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IReasonRepository
{
    Task<List<MastReasonResponse>> list(MastReasonRequest req);

    Task<ActionResult> create(MastReasonRequest req);
    Task<MastReasonResponse?> GetByCode(bool check, string name);
}