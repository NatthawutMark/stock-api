
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
public class MastGroupController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public MastGroupController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastGroup")]
    public async Task<ActionResult<List<MastGroupResponse>>> Get(MastGroupRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;
            
            List<MastGroupResponse> res = await _unitOfWork.Groups.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastGroup")]
    public async Task<ActionResult> Create([FromBody] MastGroupRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.nameTh) && string.IsNullOrEmpty(req.nameEn))
            {
                return return200(false, null, "No data To Create", "Name is empty");
            }

            MastGroupResponse? checkDuplicate = await _unitOfWork.Groups.GetByName(true, req.nameTh);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Name is already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.Groups.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastGroup Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastGroup Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastGroup Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastGroup", error = ex.Message });
        }
    }


    [HttpGet("getByName", Name = "GetMastGroupByName")]
    public async Task<ActionResult> GetByName([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastGroupResponse? res = await _unitOfWork.Groups.GetByName(false, name);
            if (res != null)
            {
                return return200(true, res, "Get MastGroup Success", "");
            }
            else
            {
                return return200(false, null, "MastGroup Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastGroup", error = ex.Message });
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
