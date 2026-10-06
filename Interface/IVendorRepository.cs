using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IVendorRepository
{
    Task<List<MastVendorResponse>> list(MastVendorRequest req);

    Task<ActionResult> create(MastVendorRequest req);
    Task<MastVendorResponse?> GetByCode(bool check, string vendorCode);
}