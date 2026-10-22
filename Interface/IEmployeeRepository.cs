using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.Response.MasterResponse;

namespace stock_api.Interfaces;

public interface IEmployeeRepository
{
    Task<List<MastEmployeeResponse>> list(MastEmployeeRequest req);
    Task<ActionResult> create(MastEmployeeRequest req);
    Task<ActionResult> update(MastEmployeeRequest req);
    Task<ActionResult> delete(string id);
    Task<MastEmployeeResponse?> GetByCode(bool check, string empCode);
    Task<MastEmployeeResponse?> GetById(string id);
}

