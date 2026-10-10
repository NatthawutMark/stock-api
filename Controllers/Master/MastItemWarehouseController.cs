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
public class MastItemWarehouseController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public MastItemWarehouseController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastItemWarehouse")]
    public async Task<ActionResult<List<MastItemWarehouseResponse>>> Get(MastItemWarehouseRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastItemWarehouseResponse> res = await _unitOfWork.ItemWarehouses.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastItemWarehouse")]
    public async Task<ActionResult> Create([FromBody] MastItemWarehouseRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.itemId) || string.IsNullOrEmpty(req.warehouseId))
            {
                return return200(false, null, "No data To Create", "itemId or warehouseId is empty");
            }

            MastItemWarehouseResponse? checkDuplicate = await _unitOfWork.ItemWarehouses.GetByItemIdAndWarehouseId(req.itemId, req.warehouseId);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Item Warehouse mapping already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.ItemWarehouses.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastItemWarehouse Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastItemWarehouse Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastItemWarehouse Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastItemWarehouse", error = ex.Message });
        }
    }

    [HttpGet("getByItemIdAndWarehouseId", Name = "GetMastItemWarehouseByItemIdAndWarehouseId")]
    public async Task<ActionResult> GetByItemIdAndWarehouseId([FromQuery] string itemId, [FromQuery] string warehouseId)
    {
        try
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(warehouseId))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastItemWarehouseResponse? res = await _unitOfWork.ItemWarehouses.GetByItemIdAndWarehouseId(itemId, warehouseId);
            if (res != null)
            {
                return return200(true, res, "Get MastItemWarehouse Success", "");
            }
            else
            {
                return return200(false, null, "MastItemWarehouse Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastItemWarehouse", error = ex.Message });
        }
    }

    [HttpPost("sync", Name = "SyncMastItemWarehouse")]
    public async Task<ActionResult> Sync([FromBody] MastItemWarehouseSyncRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.itemId))
            {
                return BadRequest(new { status = false, message = "Invalid request data or missing itemId" });
            }

            string userId = _systemService.GetUserId();

            var result = await _unitOfWork.ItemWarehouses.SyncItemWarehouses(req, userId);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Sync MastItemWarehouse Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Sync MastItemWarehouse Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Sync MastItemWarehouse Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while syncing the MastItemWarehouse", error = ex.Message });
        }
    }

    [HttpGet("getByItemId", Name = "GetMastItemWarehouseByItemId")]
    public async Task<ActionResult<List<MastItemWarehouseResponse>>> GetByItemId([FromQuery] string itemId)
    {
        try
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            var res = await _unitOfWork.ItemWarehouses.GetByItemId(itemId);
            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastItemWarehouse", error = ex.Message });
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

