using Microsoft.AspNetCore.Mvc;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;

namespace stock_api.Interfaces;

public interface IDocReceiveRepository
{
    // Task<List<DocTransResponse>> List(DocTransListRequest req);
    // Task<DocTransResponse?> GetById(string id);
    // Task<DocTransResponse?> GetByDocNo(string docNo);
    Task<List<DocReceiveDetailResponse>> List(DocReceiveDetailRequest req);
    Task<List<DocReceiveDetailResponse>> getListById(string _id);
    Task<ActionResult> CreateList(List<DocReceiveDetailRequest> req);
    Task<ActionResult> Update(string id, List<DocReceiveDetailRequest> req);
    Task<ActionResult> Confirm(string id);
    Task<ActionResult> Putaway(string _id, string? userId = null);
    // Task<ActionResult> UpdateStatus(DocTransUpdateStatusRequest req, string userId);
    // Task<ActionResult> Delete(string id, string userId);
    // Task<List<DocTransLogResponse>> GetLogs(string docId);
}

