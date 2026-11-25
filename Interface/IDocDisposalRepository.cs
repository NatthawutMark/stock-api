using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;

namespace stock_api.Interfaces;

public interface IDocDisposalRepository
{
    Task<List<DocDisposalDetailResponse>> List(DocDisposalDetailRequest req);
    Task<List<DocDisposalDetailResponse>> getListById(string _id);
    Task<ActionResult> CreateList(List<DocDisposalDetailRequest> req);
    Task<ActionResult> Update(string id, List<DocDisposalDetailRequest> req);
    Task<ActionResult> Confirm(string id);
    Task<ActionResult> Picking(string _id, string? userId = null);
}

