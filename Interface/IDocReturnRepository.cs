using Microsoft.AspNetCore.Mvc;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;

namespace stock_api.Interfaces;

public interface IDocReturnRepository
{
    Task<List<DocReturnDetailResponse>> List(DocReturnDetailRequest req);
    Task<List<DocReturnDetailResponse>> getListById(string _id);
    Task<ActionResult> CreateList(List<DocReturnDetailRequest> req);
    Task<ActionResult> Update(string id, List<DocReturnDetailRequest> req);
    Task<ActionResult> Confirm(string id, string? userId = null);
    Task<ActionResult> Putaway(string _id, string? userId = null);
}

