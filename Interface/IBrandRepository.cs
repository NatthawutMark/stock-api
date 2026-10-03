using stock_api.Models;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IBrandRepository
{
    Task<List<MastBrandList>> list(MastBrandRequest req);
}