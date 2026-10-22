using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;
using static stock_api.Response.MasterResponse;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MastEmployeeController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public MastEmployeeController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastEmployee")]
    public async Task<ActionResult<List<MastEmployeeResponse>>> Get(MastEmployeeRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastEmployeeResponse> res = await _unitOfWork.Employees.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastEmployee")]
    public async Task<ActionResult> Create([FromBody] MastEmployeeRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.empCode))
            {
                return return200(false, null, "No data To Create", "Employee code is empty");
            }

            MastEmployeeResponse? checkDuplicate = await _unitOfWork.Employees.GetByCode(true, req.empCode);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Employee code is already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.Employees.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastEmployee Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastEmployee Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastEmployee Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastEmployee", error = ex.Message });
        }
    }

    [HttpGet("getByCode", Name = "GetMastEmployeeByCode")]
    public async Task<ActionResult> GetByCode([FromQuery] string Code)
    {
        try
        {
            if (string.IsNullOrEmpty(Code))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastEmployeeResponse? res = await _unitOfWork.Employees.GetByCode(false, Code);
            if (res != null)
            {
                return return200(true, res, "Get MastEmployee Success", "");
            }
            else
            {
                return return200(false, null, "MastEmployee Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastEmployee", error = ex.Message });
        }
    }

    [HttpGet("getById", Name = "GetMastEmployeeById")]
    public async Task<ActionResult> GetById([FromQuery] string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastEmployeeResponse? res = await _unitOfWork.Employees.GetById(id);
            if (res != null)
            {
                return return200(true, res, "Get MastEmployee Success", "");
            }
            else
            {
                return return200(false, null, "MastEmployee Not Found", "");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastEmployee", error = ex.Message });
        }
    }

    [HttpPost("update", Name = "UpdateMastEmployeePost")]
    public async Task<ActionResult> update([FromBody] MastEmployeeRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.id))
            {
                return return200(false, null, "No data To Update", "Employee id is required");
            }

            // Check if empCode is being updated to another employee's existing empCode
            if (!string.IsNullOrEmpty(req.empCode))
            {
                MastEmployeeResponse? checkDuplicate = await _unitOfWork.Employees.GetByCode(true, req.empCode);
                if (checkDuplicate != null && checkDuplicate.id != req.id)
                {
                    return return200(false, null, "Duplicate Data", "Employee code already exists for another employee");
                }
            }

            string? userId = _systemService.GetUserId();
            req.UpdateBy = string.IsNullOrEmpty(userId) ? "admin" : userId;

            var result = await _unitOfWork.Employees.update(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Update MastEmployee Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastEmployee Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastEmployee Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating the MastEmployee", error = ex.Message });
        }
    }

    [HttpPost("delete", Name = "DeleteMastEmployee")]
    public async Task<ActionResult> Delete([FromBody] MastEmployeeRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Employee ID is required." });
            }

            var result = await _unitOfWork.Employees.delete(req.id);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Delete MastEmployee Success", "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Delete MastEmployee Failed", "");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while deleting the MastEmployee", error = ex.Message });
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

