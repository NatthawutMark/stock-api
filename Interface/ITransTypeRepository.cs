using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface ITransTypeRepository
{
    Task<List<MastTransTypeResponse>> list(MastTransTypeRequest req);

    Task<ActionResult> create(MastTransTypeRequest req);
    Task<MastTransTypeResponse?> GetByName(bool check, string name);
}