
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
public class MastLocationController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public MastLocationController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastLocation")]
    public async Task<ActionResult<List<MastLocationResponse>>> Get(MastLocationRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastLocationResponse> res = await _unitOfWork.Locations.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastLocation")]
    public async Task<ActionResult> Create([FromBody] MastLocationRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.code))
            {
                return return200(false, null, "No data To Create", "Code and Name is empty");
            }

            MastLocationResponse? checkDuplicate = await _unitOfWork.Locations.GetByCode(true, req.code);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Code is already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.Locations.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastLocation Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastLocation Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastLocation Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastLocation", error = ex.Message });
        }
    }


    [HttpGet("getByCode", Name = "GetMastLocationByCode")]
    public async Task<ActionResult> GetByCode([FromQuery] string code)
    {
        try
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastLocationResponse? res = await _unitOfWork.Locations.GetByCode(false, code);
            if (res != null)
            {
                return return200(true, res, "Get MastLocation Success", "");
            }
            else
            {
                return return200(false, null, "MastLocation Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastLocation", error = ex.Message });
        }
    }

    [HttpPost("update", Name = "UpdateMastLocation")]
    public async Task<ActionResult> Update([FromBody] MastLocationRequest req)
    {
        try
        {
            if (req == null || (string.IsNullOrEmpty(req.id) && string.IsNullOrEmpty(req.code)))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Location ID or code is required." });
            }

            string? userId = _systemService.GetUserId();
            req.UpdateBy = string.IsNullOrEmpty(userId) ? "admin" : userId;

            var result = await _unitOfWork.Locations.update(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Update MastLocation Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastLocation Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastLocation Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating the MastLocation", error = ex.Message });
        }
    }

    [HttpPost("delete", Name = "DeleteMastLocation")]
    public async Task<ActionResult> Delete([FromBody] MastLocationRequest req)
    {
        try
        {
            if (req == null || (string.IsNullOrEmpty(req.id) && string.IsNullOrEmpty(req.code)))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Location ID or code is required." });
            }

            var targetId = !string.IsNullOrEmpty(req.id) ? req.id : req.code;
            var result = await _unitOfWork.Locations.delete(targetId);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Delete MastLocation Success", "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Delete MastLocation Failed", "");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while deleting the MastLocation", error = ex.Message });
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
