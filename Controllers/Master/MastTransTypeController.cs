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
public class MastTransTypeController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public MastTransTypeController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    // [HttpPost("list", Name = "ListMastTransType")]
    // public async Task<ActionResult<List<MastTransTypeResponse>>> Get(MastTransTypeRequest req)
    // {
    //     try
    //     {
    //         req.isDelete = false;

    //         List<MastTransTypeResponse> res = await _unitOfWork.TransTypes.list(req);

    //         return StatusCode(200, new { success = true, results = res, message = "", error = "" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(200, new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
    //     }
    // }

    [HttpPost("create", Name = "CreateMastTransType")]
    public async Task<ActionResult> Create([FromBody] MastTransTypeRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.nameTh))
            {
                return return200(false, null, "No data To Create", "Transaction type name is empty");
            }

            MastTransTypeResponse? checkDuplicate = await _unitOfWork.TransTypes.GetByName(true, req.nameTh);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Transaction type name is already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.TransTypes.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastTransType Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastTransType Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastTransType Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastVendor", error = ex.Message });
        }
    }


    // [HttpGet("getByCode", Name = "GetMastTransTypeByCode")]
    // public async Task<ActionResult> GetByCode([FromQuery] string Code)
    // {
    //     try
    //     {
    //         if (string.IsNullOrEmpty(Code))
    //         {
    //             return BadRequest(new { status = false, message = "Invalid request data" });
    //         }

    //         MastTransTypeResponse? res = await _unitOfWork.TransTypes.GetByCode(false, Code);
    //         if (res != null)
    //         {
    //             return return200(true, res, "Get MastTransType Success", "");
    //         }
    //         else
    //         {
    //             return return200(false, null, "MastTransType Not Found", "");
    //         }

    //     }
    //     catch (Exception ex)
    //     {
    //         _unitOfWork.Dispose();
    //         return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastTransType", error = ex.Message });
    //     }
    // }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}
