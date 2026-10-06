using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface ICustomerRepository
{
    Task<List<MastCustomerResponse>> list(MastCustomerRequest req);

    Task<ActionResult> create(MastCustomerRequest req);
    Task<MastCustomerResponse?> GetByCode(bool check, string custCode);
}