using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IEmployeeRepository
{
    Task<List<MastEmployeeResponse>> list(MastEmployeeRequest req);
    Task<ActionResult> create(MastEmployeeRequest req);
    Task<MastEmployeeResponse?> GetByCode(bool check, string empCode);
}

