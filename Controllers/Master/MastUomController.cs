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
public class MastUomController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public MastUomController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastUom")]
    public async Task<ActionResult<List<MastUomResponse>>> Get([FromBody] MastUomRequest? req)
    {
        try
        {
            req ??= new MastUomRequest();
            req.isDelete = false;

            List<MastUomResponse> res = await _unitOfWork.Uoms.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastUom")]
    public async Task<ActionResult> Create([FromBody] MastUomRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.name))
            {
                return return200(false, null, "No data To Create", "Name is empty");
            }

            MastUomResponse? checkDuplicate = await _unitOfWork.Uoms.GetByName(true, req.name);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Name is already exists");
            }

            string? userId = _systemService.GetUserId();
            userId = string.IsNullOrEmpty(userId) ? "admin" : userId;

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.Uoms.create(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastUom Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastUom Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastUom Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastUom", error = ex.Message });
        }
    }

    [HttpPost("update", Name = "UpdateMastUom")]
    public async Task<ActionResult> Update([FromBody] MastUomRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. UOM ID is required." });
            }

            string? userId = _systemService.GetUserId();
            req.UpdateBy = string.IsNullOrEmpty(userId) ? "admin" : userId;

            var result = await _unitOfWork.Uoms.update(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Update MastUom Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastUom Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastUom Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating the MastUom", error = ex.Message });
        }
    }

    [HttpPost("delete", Name = "DeleteMastUom")]
    public async Task<ActionResult> Delete([FromBody] MastUomRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. UOM ID is required." });
            }

            var result = await _unitOfWork.Uoms.delete(req.id);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Delete MastUom Success", "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Delete MastUom Failed", "");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while deleting the MastUom", error = ex.Message });
        }
    }

    [HttpGet("getByName", Name = "GetMastUomByName")]
    public async Task<ActionResult> GetByName([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastUomResponse? res = await _unitOfWork.Uoms.GetByName(false, name);
            if (res != null)
            {
                return return200(true, res, "Get MastUom Success", "");
            }
            else
            {
                return return200(false, null, "MastUom Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastUom", error = ex.Message });
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
