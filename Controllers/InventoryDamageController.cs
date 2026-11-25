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
public class InventoryDamageController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public InventoryDamageController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("listAll", Name = "ListAllInventoryDamage")]
    public async Task<ActionResult<List<InventoryDmgList>>> ListAll([FromBody] InventoryDmgRequest req)
    {
        try
        {
            req ??= new InventoryDmgRequest();
            req.isDelete = false;

            List<InventoryDmgList> res = await _unitOfWork.InventoryDamages.ListAll(req);
            if (res != null)
            {
                int total = await _unitOfWork.InventoryDamages.GetTotal(req);
                return return200(true, new { data = res, total }, "Fecth Success", "");
            }
            else
            {
                return StatusCode(500, new { success = false, results = "", message = "can't fecth Data", error = "" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("list", Name = "ListInventoryDamageByFilter")]
    public async Task<ActionResult<List<InventoryDmgList>>> List([FromBody] InventoryDmgRequest req)
    {
        try
        {
            req ??= new InventoryDmgRequest();
            req.isDelete = false;

            List<InventoryDmgList> res = await _unitOfWork.InventoryDamages.List(req);
            if (res != null)
            {
                int total = await _unitOfWork.InventoryDamages.GetTotal(req);
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

    [HttpGet("getTotal", Name = "GetInventoryDamageTotal")]
    public async Task<ActionResult> GetTotal([FromQuery] string? menuId = null)
    {
        try
        {
            int total = await _unitOfWork.InventoryDamages.GetTotal(new InventoryDmgRequest());
            return return200(true, new { total }, "Get InventoryDamage total success", "");
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

