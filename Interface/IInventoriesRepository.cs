using Microsoft.AspNetCore.Mvc;
using stock_api.Request;
using static stock_api.Request.InventoryRequest;
using static stock_api.Response.InventoryResponse;

namespace stock_api.Interfaces;

public interface IInventoriesRepository
{
    Task<List<InventoryList>> ListAll(InventoryRequest req);
    Task<List<InventoryList>> List(InventoryRequest req);
    Task<List<IssueInventoryList>> ListForIssue(IssueInventoryRequest req);
    Task<int> GetTotal(InventoryRequest req);
}
