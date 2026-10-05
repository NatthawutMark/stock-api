
using stock_api.dbo;
using stock_api.Models;
using static stock_api.response.MasterResponse;
using static stock_api.Request.MasterRequest;
namespace stock_api.Interfaces;

public interface IItemRepository
{
    Task<List<MastItemResponse>> GetAll(MastItemRequest req);
}