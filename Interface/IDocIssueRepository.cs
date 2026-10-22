using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;

namespace stock_api.Interfaces;

public interface IDocIssueRepository
{
    Task<List<DocIssueDetailResponse>> List(DocIssueDetailRequest req);
    Task<List<DocIssueDetailResponse>> getListById(string _id);
    Task<ActionResult> CreateList(List<DocIssueDetailRequest> req);
    Task<ActionResult> Update(string id, List<DocIssueDetailRequest> req);
    Task<ActionResult> Confirm(string id);
    Task<ActionResult> Picking(string _id, string? userId = null);
}

