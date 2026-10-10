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
public class MastItemController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public MastItemController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastItem")]
    public async Task<ActionResult> Get([FromBody] MastItemRequest? req)
    {
        try
        {
            req ??= new MastItemRequest();

            List<MastItemResponse> res = await _unitOfWork.Items.GetAll(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("getByCode", Name = "GetMastItemByCode")]
    public async Task<ActionResult> GetByCode([FromQuery] string code)
    {
        try
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { status = false, message = "Item code is required" });
            }

            var res = await _unitOfWork.Items.GetByCode(code);
            if (res != null)
            {
                return return200(true, res, "Get MastItem Success", "");
            }
            return return200(false, null, "MastItem Not Found", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = false, message = ex.Message, error = ex.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastItem")]
    public async Task<ActionResult> Create([FromBody] MastItemRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.itemCode) || string.IsNullOrEmpty(req.itemName))
            {
                return return200(false, null, "Validation Error", "ItemCode and ItemName are required");
            }

            var existing = await _unitOfWork.Items.GetByCode(req.itemCode);
            if (existing != null)
            {
                return return200(false, null, "Duplicate Data", "Item code already exists");
            }

            string? userId = _systemService.GetUserId();
            userId = string.IsNullOrEmpty(userId) ? "admin" : userId;

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            // Normalize empty foreign keys to null
            req.brandId = string.IsNullOrWhiteSpace(req.brandId) ? null : req.brandId;
            req.locationId = string.IsNullOrWhiteSpace(req.locationId) ? null : req.locationId;
            req.uomId = string.IsNullOrWhiteSpace(req.uomId) ? null : req.uomId;

            var result = await _unitOfWork.Items.create(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastItem Success", "");
            }
            else if (result is BadRequestObjectResult bad)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastItem Failed", bad.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastItem Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastItem", error = ex.Message });
        }
    }

    [HttpPost("update", Name = "UpdateMastItem")]
    public async Task<ActionResult> Update([FromBody] MastItemRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Item ID is required." });
            }

            if (!string.IsNullOrEmpty(req.originalItemCode) && !string.Equals(req.originalItemCode, req.itemCode, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _unitOfWork.Items.GetByCode(req.itemCode!);
                if (existing != null && existing.id != req.id)
                {
                    return return200(false, null, "Duplicate Data", "Item code already exists");
                }
            }

            string? userId = _systemService.GetUserId();
            req.UpdateBy = string.IsNullOrEmpty(userId) ? "admin" : userId;

            // Normalize empty foreign keys to null
            req.brandId = string.IsNullOrWhiteSpace(req.brandId) ? null : req.brandId;
            req.locationId = string.IsNullOrWhiteSpace(req.locationId) ? null : req.locationId;
            req.uomId = string.IsNullOrWhiteSpace(req.uomId) ? null : req.uomId;

            var result = await _unitOfWork.Items.update(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Update MastItem Success", "");
            }
            else if (result is BadRequestObjectResult bad)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastItem Failed", bad.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastItem Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating the MastItem", error = ex.Message });
        }
    }

    [HttpPost("delete", Name = "DeleteMastItem")]
    public async Task<ActionResult> Delete([FromBody] MastItemRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Item ID is required." });
            }

            var result = await _unitOfWork.Items.delete(req.id);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Delete MastItem Success", "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Delete MastItem Failed", "");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while deleting the MastItem", error = ex.Message });
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
