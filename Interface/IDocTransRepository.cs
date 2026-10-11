using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;

namespace stock_api.Interfaces;

public interface IDocTransRepository
{
    Task<List<DocTransResponse>> List(DocTransListRequest req);
    Task<DocTransResponse?> GetById(string id);
    Task<DocTransResponse?> GetByDocNo(string docNo);
    Task<ActionResult> Create(DocTransCreateRequest req);
    Task<ActionResult> Update(DocTransUpdateRequest req);
    Task<ActionResult> UpdateStatus(DocTransUpdateStatusRequest req, string userId);
    Task<ActionResult> Delete(string id, string userId);
    Task<List<DocTransLogResponse>> GetLogs(string docId);
}

