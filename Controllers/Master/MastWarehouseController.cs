using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MastWarehouseController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public MastWarehouseController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastWarehouse")]
    public async Task<ActionResult> List([FromBody] MastWarehouseRequest? req)
    {
        try
        {
            req ??= new MastWarehouseRequest();
            req.isDelete = false;

            var res = await _unitOfWork.Warehouses.list(req);
            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("getByCode", Name = "GetMastWarehouseByCode")]
    public async Task<ActionResult> GetByCode([FromQuery] string code)
    {
        try
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { status = false, message = "Code is required" });
            }

            var res = await _unitOfWork.Warehouses.GetByCodeAsync(code);
            if (res != null)
            {
                return return200(true, res, "Get MastWarehouse Success", "");
            }
            return return200(false, null, "MastWarehouse Not Found", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = false, message = ex.Message, error = ex.Message });
        }
    }

    [HttpGet("{id}", Name = "GETMastWarehouse")]
    public async Task<ActionResult> GetById(string id)
    {
        var mastWarehouses = await _unitOfWork.Warehouses.GetByIdAsync(id);
        return StatusCode(200, mastWarehouses);
    }

    [HttpPost("create", Name = "CreateMastWarehouse")]
    public async Task<ActionResult> Create([FromBody] MastWarehouseRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.Code) || string.IsNullOrEmpty(req.WarehouseName))
            {
                return return200(false, null, "Validation Error", "Code and WarehouseName are required");
            }

            var existing = await _unitOfWork.Warehouses.GetByCodeAsync(req.Code);
            if (existing != null)
            {
                return return200(false, null, "Duplicate Data", "Warehouse code already exists");
            }

            string? userId = _systemService.GetUserId();
            userId = string.IsNullOrEmpty(userId) ? "admin" : userId;

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.Warehouses.create(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastWarehouse Success", "");
            }
            else if (result is BadRequestObjectResult bad)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastWarehouse Failed", bad.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastWarehouse Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastWarehouse", error = ex.Message });
        }
    }

    [HttpPost("update", Name = "UpdateMastWarehouse")]
    public async Task<ActionResult> Update([FromBody] MastWarehouseRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Warehouse ID is required." });
            }

            if (!string.IsNullOrEmpty(req.originalCode) && !string.Equals(req.originalCode, req.Code, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _unitOfWork.Warehouses.GetByCodeAsync(req.Code);
                if (existing != null && existing.id != req.id)
                {
                    return return200(false, null, "Duplicate Data", "Warehouse code already exists");
                }
            }

            string? userId = _systemService.GetUserId();
            req.UpdateBy = string.IsNullOrEmpty(userId) ? "admin" : userId;

            var result = await _unitOfWork.Warehouses.update(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Update MastWarehouse Success", "");
            }
            else if (result is BadRequestObjectResult bad)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastWarehouse Failed", bad.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastWarehouse Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating the MastWarehouse", error = ex.Message });
        }
    }

    [HttpPost("delete", Name = "DeleteMastWarehouse")]
    public async Task<ActionResult> Delete([FromBody] MastWarehouseRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Warehouse ID is required." });
            }

            var result = await _unitOfWork.Warehouses.delete(req.id);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Delete MastWarehouse Success", "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Delete MastWarehouse Failed", "");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while deleting the MastWarehouse", error = ex.Message });
        }
    }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}
