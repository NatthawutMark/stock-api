using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using stock_api.Request;
using static stock_api.Response.InventoryResponse;
using static stock_api.Request.InventoryRequest;
using System.Net;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public InventoryController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("listAll", Name = "ListAllInventory")]
    public async Task<ActionResult<List<InventoryList>>> ListAll([FromBody] InventoryRequest req)
    {
        try
        {
            req ??= new InventoryRequest();
            req.isDelete = false;

            List<InventoryList> res = await _unitOfWork.Inventories.ListAll(req);
            if (res != null)
            {
                int total = await _unitOfWork.Inventories.GetTotal(req);
                return return200(true, new { data = res, total }, "Fecth Success", "");
            }
            else
            {
                return StatusCode(500, new { success = false, results = "", message = "can't fecth Data", error = "" });
            }
            // int total = await _unitOfWork.DocTrans.GetTotal(req.menuId);
            // return return200(true, new { data = res, total }, "Get DocTrans list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("list", Name = "ListInventory")]
    public async Task<ActionResult<List<InventoryList>>> List([FromBody] InventoryRequest req)
    {
        try
        {
            req ??= new InventoryRequest();
            req.isDelete = false;

            List<InventoryList> res = await _unitOfWork.Inventories.List(req);
            if (res != null)
            {
                int total = await _unitOfWork.Inventories.GetTotal(req);
                return return200(true, new { data = res, total }, "Fecth Success", "");
            }
            else
            {
                return StatusCode(500, new { success = false, results = "", message = "can't fecth Data", error = "" });
            }
            // int total = await _unitOfWork.DocTrans.GetTotal(req.menuId);
            // return return200(true, new { data = res, total }, "Get DocTrans list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("listForIssue", Name = "ListForIssue")]
    public async Task<ActionResult<List<IssueInventoryList>>> ListForIssue([FromBody] IssueInventoryRequest req)
    {
        try
        {

            if (string.IsNullOrEmpty(req.menuId))
                return return200(false, "", "Haven't Menu ID to Find.", "Check Menu ID");

            List<IssueInventoryList> result = await _unitOfWork.Inventories.ListForIssue(req);

            if (result != null && result.Count() > 0)
            {
                return return200(true, new { data = result }, "", "");
            }
            else
            {
                return return200(false, "", "error to call list", "");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, data = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("getTotal", Name = "GetInventoryTotal")]
    public async Task<ActionResult> GetTotal([FromQuery] string? menuId = null)
    {
        try
        {
            int total = await _unitOfWork.DocTrans.GetTotal(menuId);
            return return200(true, new { total }, "Get DocTrans total success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object? results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}

